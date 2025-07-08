using System;

namespace Academix.Domain.Entities
{
    public class Skill : BaseEntity
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
         
        // Navigation property
        public ICollection<StudentSkill> StudentSkills { get; set; }
    }

    
} 