using System.Collections.Generic;

namespace Academix.Domain.Entities
{
    public class AgeGroup : BaseEntity
    {
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }

        public virtual ICollection<TeacherAgeGroup> TeacherAgeGroups { get; set; } = new List<TeacherAgeGroup>();
    }
} 