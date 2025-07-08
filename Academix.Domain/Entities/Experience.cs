using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities
{
    public class Experience : BaseEntity
    {
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }

        public virtual ICollection<StudentExperience> StudentExperiences { get; set; } = new List<StudentExperience>();
    }
    
}