using System;

namespace Academix.Domain.Entities
{
    public class TeacherSkill : BaseEntity
    {
        public Guid TeacherId { get; set; }
        public Guid SkillId { get; set; }
        

        // Navigation properties
        public virtual Teacher Teacher { get; set; } = null!;
        public virtual Skill Skill { get; set; } = null!;
    }
} 