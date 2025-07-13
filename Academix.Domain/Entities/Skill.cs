using System;

namespace Academix.Domain.Entities
{
    public class Skill : BaseEntity
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
         
        // Navigation properties
        public ICollection<StudentSkill> StudentSkills { get; set; }
        public ICollection<TeacherSkill> TeacherSkills { get; set; }
    }

    
} 