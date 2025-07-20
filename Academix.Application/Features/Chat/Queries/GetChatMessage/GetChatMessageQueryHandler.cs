using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Academix.Application.Features.Chat.Queries.GetChatMessage
{
    public class GetChatMessageQueryHandler : IRequestHandler<GetChatMessageQuery, Result<List<ChatMessageDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetChatMessageQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<ChatMessageDto>>> Handle(GetChatMessageQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var messages = await _unitOfWork.Repository<ChatMessage>().GetAllAsync();

                var filteredMessages = await messages
                    .Where(m =>
                        m.SenderId == request.CurrentUserId && m.ReceiverId == request.OtherUserId ||
                        m.SenderId == request.OtherUserId && m.ReceiverId == request.CurrentUserId)
                    .OrderBy(m => m.SentAt)
                    .Select(m => new ChatMessageDto
                    {
                        Id = m.Id,
                        SenderId = m.SenderId,
                        ReceiverId = m.ReceiverId,
                        MessageText = m.MessageText,
                        SentAt = m.SentAt,
                        IsRead = m.IsRead,
                        ReadAt = m.ReadAt
                    })
                    .ToListAsync(cancellationToken);

                if (filteredMessages == null || !filteredMessages.Any())
                {
                    return Result<List<ChatMessageDto>>.Failure("No messages found between the selected users.");
                }

                return Result<List<ChatMessageDto>>.Success(filteredMessages);
            }
            catch (Exception ex)
            {
                return Result<List<ChatMessageDto>>.Failure($"GetChatMessagesFailed: {ex.Message}");
            }
        }
    }
}
