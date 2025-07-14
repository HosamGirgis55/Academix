using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using MediatR;

namespace Academix.Application.Features.Sessions.Commands.AcceptSessionRequest
{
    public class AcceptSessionRequestCommand : ICommand<Result<Guid>>
    {
        public Guid SessionRequestId { get; set; }
        public Guid TeacherId { get; set; }
        public DateTime ScheduledStartTime { get; set; }
    }
} 