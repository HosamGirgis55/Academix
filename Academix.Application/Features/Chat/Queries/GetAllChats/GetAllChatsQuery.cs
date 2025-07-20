using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Chat.Queries.GetAllChats
{
    public class GetAllChatsQuery : IRequest<Result<List<AllChatsDto>>>
    {
        public string UserId { get; set; }
    }
}
