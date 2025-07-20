using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.Interfaces
{
    public interface IChatHubService
    {
        Task NotifyMessagesReadAsync(string userId, string byUserId);
        Task MarkMessagesAsRead(string currentUserId, string otherUserId);
        Task SendMessage(string receiverId, string senderId, string message);
    }
}
