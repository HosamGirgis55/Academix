using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.Entities
{
    public class ChatMessage
    {
        public Guid Id { get; set; }

        public string SenderId { get; set; } // FK to ApplicationUser
        public string ReceiverId { get; set; } // FK to ApplicationUser

        public string MessageText { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }

        public ApplicationUser Sender { get; set; }
        public ApplicationUser Receiver { get; set; }
    }

}
