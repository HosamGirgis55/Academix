using System;

namespace Academix.Domain.Entities
{
    public class StudentExperience : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Guid ExperienceId { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Navigation properties
        public virtual Student Student { get; set; } = null!;
        public virtual Experience Experience { get; set; } = null!;
    }
} 