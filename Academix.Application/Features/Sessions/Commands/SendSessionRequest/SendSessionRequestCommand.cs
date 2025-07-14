using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using MediatR;

namespace Academix.Application.Features.Sessions.Commands.SendSessionRequest
{
    public class SendSessionRequestCommand : ICommand<Result<Guid>>
    {
        public Guid StudentId { get; set; }
        public Guid TeacherId { get; set; }
        public int PointsAmount { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int EstimatedDurationMinutes { get; set; }
        public DateTime RequestedDateTime { get; set; }
    }
} 