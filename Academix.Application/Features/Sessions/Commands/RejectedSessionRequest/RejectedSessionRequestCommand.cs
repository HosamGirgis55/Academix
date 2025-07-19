using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Sessions.Commands.RejectedSessionRequest
{
    public class RejectedSessionRequestCommand : IRequest<Result<Guid>>
    {
        public Guid SessionRequestId { get; set; }
        public Guid TeacherId { get; set; }
    }
}
