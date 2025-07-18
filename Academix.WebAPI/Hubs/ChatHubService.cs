using Academix.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Academix.WebAPI.Hubs
{
    public class ChatHubService : IChatHubService
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatHubService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyMessagesReadAsync(string userId, string byUserId)
        {
            await _hubContext.Clients
                .User(userId)
                .SendAsync("MessagesMarkedAsRead", new
                {
                    byUserId,
                    at = DateTime.UtcNow
                });
        }
    }

}
