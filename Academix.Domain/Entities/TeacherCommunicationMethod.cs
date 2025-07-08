using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academix.Domain.Entities
{
    public class TeacherCommunicationMethod : BaseEntity
    {
        public Guid TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; } = null!;

        public Guid CommunicationMethodId { get; set; }
        [ForeignKey("CommunicationMethodId")]
        public CommunicationMethod CommunicationMethod { get; set; } = null!;
    }
} 