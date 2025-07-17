using Academix.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Chat.Commands
{
    public class SendMessageCommand : IRequest<Result<Guid>>
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string MessageText { get; set; }
    }
}
