using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using CB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;

namespace CB.Client.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : ControllerBase
    {
        private string _baseAddress = "http://localhost:5000/home/ten";
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        private static Polly.CircuitBreaker.AsyncCircuitBreakerPolicy<HttpResponseMessage> circuitBreaker =
            Policy.HandleResult<HttpResponseMessage>(x => !x.IsSuccessStatusCode).Or<HttpRequestException>()
            .CircuitBreakerAsync(2, TimeSpan.FromSeconds(10),
                (a, b) => { Console.WriteLine("Break"); },
                () => { Console.WriteLine("Reset"); },
                () => { Console.WriteLine("Half"); });
        private readonly AsyncFallbackPolicy<HttpResponseMessage> fallbackPolicy;

        public HomeController()
        {
            _retryPolicy = Policy.HandleResult<HttpResponseMessage>(result => !result.IsSuccessStatusCode)
                .RetryAsync(2, (a, c) => { /*log*/ });

            fallbackPolicy = Policy.HandleResult<HttpResponseMessage>(result => !result.IsSuccessStatusCode)
                .Or<BrokenCircuitException>()
                .FallbackAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new  ObjectContent(typeof(Message),new Message
                    {
                        Id=1000,
                        Text = "default text"
                    },new JsonMediaTypeFormatter())
                });
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            HttpClient client = new HttpClient();
            var result = await fallbackPolicy.ExecuteAsync(()=>_retryPolicy.ExecuteAsync(()=>circuitBreaker.ExecuteAsync(()=>client.GetAsync(_baseAddress))));
            var str = await result.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<ClientMessage>(str);
            return Ok(obj);
        }
    }
}
