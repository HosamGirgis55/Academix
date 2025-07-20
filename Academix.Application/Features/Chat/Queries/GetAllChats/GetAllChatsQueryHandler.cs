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

                foreach (var Id in lastMessagesPerUser)
                {

                          
                        var x = new AllChatsDto 
                        {
                            SenderId = Id.SenderId,
                            ReceiverId = Id.ReceiverId,
                            SenderName = Id.Sender.FirstName + " " + Id.Sender.LastName,
                            ReceiverName = Id.Receiver.FirstName + " " + Id.Receiver.LastName,
                            LastMessage = Id.MessageText,
                            SentAt = Id.CreatedAt,
                            ProfilePictureUrl = request.UserId == Id.SenderId? Id.Receiver.ProfilePictureUrl: Id.Sender.ProfilePictureUrl ?? "",
                        };

                
                        chats.Add(x);

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
