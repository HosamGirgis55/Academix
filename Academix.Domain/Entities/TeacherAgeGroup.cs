using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academix.Domain.Entities
{
    public class TeacherAgeGroup : BaseEntity
    {
        public Guid TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; } = null!;

        public Guid AgeGroupId { get; set; }
        [ForeignKey("AgeGroupId")]
        public AgeGroup AgeGroup { get; set; } = null!;
    }
} 