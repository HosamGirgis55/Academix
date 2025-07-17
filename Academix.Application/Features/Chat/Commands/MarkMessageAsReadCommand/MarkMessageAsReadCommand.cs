using Academix.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Chat.Commands.MakeMessageAsReadCommand
{
    public class MarkMessageAsReadCommand : IRequest<Result>
    {
        public string CurrentUserId { get; set; } = default!;
        public string OtherUserId { get; set; } = default!;
    }

}
