using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.DTOs
{
    public class ChatMessageDto
    {
        public Guid Id { get; set; }

        public string SenderId { get; set; } 
        public string ReceiverId { get; set; } 

        public string MessageText { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
    }
}
