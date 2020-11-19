using System;

namespace CB.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class ClientMessage : Message
    {
        public string ClientText { get; set; } = "This is Client Text";
    }
}
