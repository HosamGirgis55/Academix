using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Chat.Queries.GetAllChats
{
    internal class GetAllChatsQueryHandler : IRequestHandler<GetAllChatsQuery, Result<List<AllChatsDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllChatsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<List<AllChatsDto>>> Handle(GetAllChatsQuery request, CancellationToken cancellationToken)
        {
            try
            {

                
                var chatsQuery = await _unitOfWork.Repository<ChatMessage>().GetAllAsync();
                var messages = await chatsQuery
                    .Where(ch => ch.SenderId == request.UserId || ch.ReceiverId == request.UserId)
                    .OrderByDescending(ch => ch.CreatedAt) // عشان نختار أحدث رسالة
                    .Include(ch => ch.Sender)
                    .Include(ch => ch.Receiver)
                    .ToListAsync(cancellationToken);

                // استخدم DistinctBy لإرجاع رسالة واحدة فقط لكل شخص اتكلم معاه
                var lastMessagesPerUser = messages
                    .DistinctBy(ch => ch.SenderId == request.UserId ? ch.ReceiverId : ch.SenderId)
                    .ToList();
                if (lastMessagesPerUser == null)
                {
                    return Result<List<AllChatsDto>>.Failure("No chats found ");
                }

                List<AllChatsDto> chats = new List<AllChatsDto>();

                foreach (var chat in lastMessagesPerUser)
                {
                    var dto = new AllChatsDto
                    {
                        SenderId = request.UserId,
                        ReceiverId = chat.SenderId == request.UserId ? chat.ReceiverId : chat.SenderId,
                        SenderName = chat.SenderId == request.UserId ? $"{chat.Sender.FirstName} {chat.Sender.LastName}" : $"{chat.Receiver.FirstName} {chat.Receiver.LastName}", // Use the current user's name if available
                        ReceiverName = chat.SenderId == request.UserId
                            ? $"{chat.Receiver.FirstName} {chat.Receiver.LastName}"
                            : $"{chat.Sender.FirstName} {chat.Sender.LastName}",
                        LastMessage = chat.MessageText,
                        SentAt = chat.CreatedAt,
                        ProfilePictureUrl = chat.SenderId == request.UserId
                            ? chat.Receiver.ProfilePictureUrl ?? ""
                            : chat.Sender.ProfilePictureUrl ?? ""
                    };

                    chats.Add(dto);
                }
                return Result<List<AllChatsDto>>.Success(chats);

            }
            catch (Exception ex) 
            {
                return Result<List<AllChatsDto>>.Failure($"GetAllChatsFailed: {ex.Message}");

            }
        }
    
    
    
    }
}
