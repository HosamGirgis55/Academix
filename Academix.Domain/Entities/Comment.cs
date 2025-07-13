using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academix.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public int Rating { get; set; } // 1-5 star rating
        
        // Foreign Keys
        public Guid StudentId { get; set; }
        public Guid TeacherId { get; set; }
        
        // Navigation Properties
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;
        
        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; } = null!;
    }
} 