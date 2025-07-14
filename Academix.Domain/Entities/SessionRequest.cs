using System.ComponentModel.DataAnnotations.Schema;
using Academix.Domain.Enums;

namespace Academix.Domain.Entities
{
    public class SessionRequest : BaseEntity
    {
        // Foreign Keys
        public Guid StudentId { get; set; }
        public Guid TeacherId { get; set; }
        
        // Navigation Properties
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;
        
        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; } = null!;
        
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
        
        // Related Session
        public Guid? SessionId { get; set; }
        [ForeignKey("SessionId")]
        public virtual Session? Session { get; set; }
    }
} 