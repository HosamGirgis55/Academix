using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using Academix.Infrastructure.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Academix.WebAPI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatHub(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task SendMessage(string receiverId, string message)
        {
            var senderId = Context.UserIdentifier;

            var chatMessage = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                MessageText = message,
                SentAt = DateTime.UtcNow,
                IsRead = false 
            };

            await _unitOfWork.ChatMessage.AddAsync(chatMessage);
            await _unitOfWork.SaveChangesAsync();


            await Clients.User(receiverId).SendAsync("ReceiveMessage", new
            {
                SenderId = senderId,
                Content = message,
                SentAt = chatMessage.SentAt
            });

            await Clients.User(senderId).SendAsync("MessageSent", new
            {
                ReceiverId = receiverId,
                Content = message,
                SentAt = chatMessage.SentAt
            });
        }
    }
}
