using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities
{
    public class TeachingArea : BaseEntity
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
 
        // Navigation properties
        public ICollection<TeacherTeachingArea> TeacherTeachingAreas { get; set; } = new List<TeacherTeachingArea>();
    }
} 