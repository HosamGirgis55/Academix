using System;

namespace Academix.Domain.Entities
{
    public class GraduationStatus : BaseEntity
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
         
        // Navigation property
        public ICollection<Student> Students { get; set; }
    }
} 