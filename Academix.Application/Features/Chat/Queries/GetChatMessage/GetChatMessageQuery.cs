using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Chat.Queries.GetChatMessage
{
    public class GetChatMessageQuery : IRequest<Result<List<ChatMessageDto>>>
    {
        public string CurrentUserId { get; set; }
        public string OtherUserId { get; set; }
    }
}
