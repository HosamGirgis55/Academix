using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academix.Application.Features.Chat.Commands.MakeMessageAsReadCommand
{
    public class MarkMessageAsReadCommandHandler : IRequestHandler<MarkMessageAsReadCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatHubService _chatHubService;

        public MarkMessageAsReadCommandHandler(IUnitOfWork unitOfWork, IChatHubService chatHubService)
        {
            _unitOfWork = unitOfWork;
            _chatHubService = chatHubService;
        }

        public async Task<Result> Handle(MarkMessageAsReadCommand request, CancellationToken cancellationToken)
        {
            var messageRepo = await _unitOfWork.Repository<ChatMessage>().GetAllAsync();

            var unreadMessages = await messageRepo
                .Where(m =>
                    m.ReceiverId == request.CurrentUserId &&
                    m.SenderId == request.OtherUserId &&
                    !m.IsRead)
                .ToListAsync(cancellationToken);

            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _chatHubService.NotifyMessagesReadAsync(request.OtherUserId, request.CurrentUserId);

            return Result.Success();
        }
    }
}
