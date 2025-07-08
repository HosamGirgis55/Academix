using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academix.Domain.Entities
{
    public class TeacherTeachingLanguage : BaseEntity
    {
        public Guid TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; } = null!;

        public Guid TeachingLanguageId { get; set; }
        [ForeignKey("TeachingLanguageId")]
        public TeachingLanguage TeachingLanguage { get; set; } = null!;
    }
} 