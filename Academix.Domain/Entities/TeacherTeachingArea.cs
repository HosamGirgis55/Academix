using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academix.Domain.Entities
{
    public class TeacherTeachingArea : BaseEntity
    {
        public Guid TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; } = null!;

        public Guid TeachingAreaId { get; set; }
        [ForeignKey("TeachingAreaId")]
        public TeachingArea TeachingArea { get; set; } = null!;
    }
} 