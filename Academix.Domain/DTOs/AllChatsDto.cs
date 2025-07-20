using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.DTOs
{
    public class AllChatsDto
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string LastMessage { get; set; }
        public string SenderName { get; set; }  
        public string ReceiverName { get; set; }
        public DateTime SentAt { get; set; }
        public string ProfilePictureUrl { get; set; }

    }
}
