using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace alcobot.service.Models
{
    public class Message
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long ChatId { get; set; }
        public string Text { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public bool IsRecognized { get; set; }
    }
}
