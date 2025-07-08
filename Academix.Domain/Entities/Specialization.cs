using System;

namespace Academix.Domain.Entities
{
    public class Specialization : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        
        // Navigation property
        public ICollection<Student> Students { get; set; }
    }
} 