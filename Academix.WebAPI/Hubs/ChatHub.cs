using Academix.Application.Features.Chat.Commands.SentMessage;
using Academix.Domain.Interfaces;
using Academix.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Academix.WebAPI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;

        public ChatHub(IMediator mediator, ApplicationDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        public override Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var x = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            Console.WriteLine("hello");
            _context.connections.AddAsync(new Domain.Entities.Connections{
              ConnectionId = connectionId,
              UserId = x
            });
            return base.OnConnectedAsync();
        }
        public async Task SendMessage(string receiverId, string message)
        {
            var senderId = Context.UserIdentifier;

            var result = await _mediator.Send(new SendMessageCommand
            {
                SenderId = senderId!,
                ReceiverId = receiverId,
                MessageText = message
            });

            if (!result.IsSuccess)
                return;

            var sentAt = DateTime.UtcNow;

            await Clients.User(receiverId).SendAsync("ReceiveMessage", new
            {
                SenderId = senderId,
                Content = message,
                SentAt = sentAt
            });

            await Clients.User(senderId!).SendAsync("MessageSent", new
            {
                ReceiverId = receiverId,
                Content = message,
                SentAt = sentAt
            });
        }

       
        public async Task MarkMessagesAsRead(string otherUserId)
        {
            var currentUserId = Context.UserIdentifier!;

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

                await Clients.User(otherUserId).SendAsync("MessagesMarkedAsRead", currentUserId);
            }
        }
    }
}
