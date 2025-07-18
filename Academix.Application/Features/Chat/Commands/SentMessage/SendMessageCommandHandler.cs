using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Chat.Commands.SentMessage
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SendMessageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var message = new ChatMessage
            {
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                MessageText = request.MessageText,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            await _unitOfWork.ChatMessage.AddAsync(message);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Success(message.Id);
        }
    }

}
