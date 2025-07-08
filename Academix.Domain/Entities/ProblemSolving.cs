using System;
using System.Collections.Generic;

namespace Academix.Domain.Entities
{
    public class Experience : BaseEntity
    {
        public required string NameAr { get; set; }
        public required string NameEn { get; set; }
        public required string Description { get; set; }

        public ICollection<StudentExperience> StudentExperiences { get; set; } = new List<StudentExperience>();
    }

    public class StudentExperience
    {
        public Guid StudentId { get; set; }
        public Student Student { get; set; }

        public Guid ExperienceId { get; set; }
        public Experience Experience { get; set; }
 
    }
} 