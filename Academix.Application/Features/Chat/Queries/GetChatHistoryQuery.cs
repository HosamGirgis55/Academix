using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Chat.Queries
{
    public class GetChatHistoryQuery : IRequest<Result<ChatMessageDto>>
    {
        public string User1Id { get; set; }
        public string User2Id { get; set; }
    }
}
