using System;

namespace Academix.Domain.Entities
{
    public class StudentSkill : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Guid SkillId { get; set; }
        public int Level { get; set; }

        // Navigation properties
        public virtual Student Student { get; set; } = null!;
        public virtual Skill Skill { get; set; } = null!;
    }
} 