using Academix.Domain.Interfaces;
using Academix.Infrastructure.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Academix.WebAPI.Hubs
{
    public class ChatHubService : IChatHubService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ApplicationDbContext _context;

        public ChatHubService(IHubContext<ChatHub> hubContext, ApplicationDbContext context)
        {
            _hubContext = hubContext;
            _context = context;
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

        public async Task SendMessage(string receiverId, string senderId, string message)
        {
            var sentAt = DateTime.UtcNow;
            var sendrConnectionId = await _context.connections.FirstOrDefaultAsync(x => x.UserId == receiverId);
            var receiverConnectionId = await _context.connections.FirstOrDefaultAsync(x => x.UserId == senderId);
            await _hubContext.Clients.Client(receiverConnectionId.ConnectionId).SendAsync("ReceiveMessage", new
            {
                SenderId = senderId,
                Content = message,
                SentAt = sentAt
            });

            await _hubContext.Clients.Client(sendrConnectionId.ConnectionId).SendAsync("MessageSent", new
            {
                ReceiverId = receiverId,
                Content = message,
                SentAt = sentAt
            });
        }

        public async Task MarkMessagesAsRead(string currentUserId, string otherUserId)
        {
            var messages = await _context.ChatMessages
                .Where(m =>
                    m.SenderId == otherUserId &&
                    m.ReceiverId == currentUserId &&
                    !m.IsRead)
                .ToListAsync();

            if (messages.Any())
            {
                foreach (var msg in messages)
                {
                    msg.IsRead = true;
                    msg.ReadAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                await _hubContext.Clients.User(otherUserId).SendAsync("MessagesMarkedAsRead", currentUserId);
            }
        }
    }
}
