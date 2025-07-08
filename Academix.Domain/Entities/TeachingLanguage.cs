using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities
{
    public class TeachingLanguage : BaseEntity
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
 
        // Navigation properties
        public ICollection<TeacherTeachingLanguage> TeacherTeachingLanguages { get; set; } = new List<TeacherTeachingLanguage>();
    }
} 