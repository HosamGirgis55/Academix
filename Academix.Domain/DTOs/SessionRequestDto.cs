using Academix.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.DTOs
{
    public class SessionRequestDto
    {
        public Guid SessionId { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public Guid TeacherId { get; set; }

        // Request Details
        public int PointsAmount { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int EstimatedDurationMinutes { get; set; }
        public DateTime RequestedDateTime { get; set; }

        // Request Status
        public SessionRequestStatus Status { get; set; } = SessionRequestStatus.Pending;
        public DateTime? AcceptedAt { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? RejectionReason { get; set; }

    }
}
