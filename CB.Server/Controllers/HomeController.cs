using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CB.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : ControllerBase
    {
        static int Counter = 0;

        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new Message { Id = 1, Text = "my text" });
        }

        [HttpGet]
        public IActionResult Odd()
        {
            Counter += 1;
            if (Counter % 2 == 0)
            {
                return Ok(new Message { Id = Counter, Text = "successfull. odd requests" });
            }
            else
            {
                return BadRequest(new Message { Id = Counter, Text = "failed. even requests" });
            }
        }

        [HttpGet]
        public IActionResult Ten()
        {
            Console.WriteLine($"counter:{Counter}");
            Counter += 1;
            if (Counter % 10 == 0)
            {
                return Ok(new Message { Id = Counter, Text = "successfull. 10X requests" });
            }
            else
            {
                return BadRequest(new Message { Id = Counter, Text = "failed. not 10X requests" });
            }
        }

        [HttpGet]
        public IActionResult Long()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            System.Threading.Thread.Sleep(20000);
            sw.Stop();
            return Ok(new Message
            {
                Id=3,
                Text = $"success. time: {sw.ElapsedMilliseconds/1000} seconds."
            });
        }

        [HttpGet]
        public IActionResult Ex()
        {
            throw new Exception("exception ocurred");            
        }

    }
}
