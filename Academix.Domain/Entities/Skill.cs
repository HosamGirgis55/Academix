using System;

namespace Academix.Domain.Entities
{
    public class Skill : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        
        // Navigation property
        public ICollection<StudentSkill> StudentSkills { get; set; }
    }

    public class StudentSkill
    {
        public Guid StudentId { get; set; }
        public Student Student { get; set; }

        public Guid SkillId { get; set; }
        public Skill Skill { get; set; }

        // Additional properties can be added here, like proficiency level
 
    }
} 