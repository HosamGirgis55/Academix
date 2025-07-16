using Academix.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.DTOs
{
    public class SessionDto
    {
        public Guid StudentId { get; set; }
        public Guid TeacherId { get; set; }
        public Guid SessionRequestId { get; set; }

        // Session Details
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PointsAmount { get; set; }

        // Timing
        public DateTime ScheduledStartTime { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public int PlannedDurationMinutes { get; set; }
        public int? ActualDurationMinutes { get; set; }

        // Session Status and Progress
        public SessionStatus Status { get; set; } = SessionStatus.Scheduled;
        public bool IsPointsTransferred { get; set; } = false;
        public DateTime? PointsTransferredAt { get; set; }

        // Session Notes and Feedback
        public string? TeacherNotes { get; set; }
        public string? StudentNotes { get; set; }
        public int? StudentRating { get; set; } // 1-5 stars
        public int? TeacherRating { get; set; } // 1-5 stars
    }
}
