using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities
{
    public class CommunicationMethod : BaseEntity
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
 
        // Navigation properties
        public ICollection<TeacherCommunicationMethod> TeacherCommunicationMethods { get; set; } = new List<TeacherCommunicationMethod>();
    }
} 