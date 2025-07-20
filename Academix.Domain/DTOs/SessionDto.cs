using Academix.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.DTOs
{
    public class SessionDto
    {
        public Guid SessionRequestId { get; set; }

        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; }

        // Session Details
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PointsAmount { get; set; }

        // Timing
        public DateTime ScheduledStartTime { get; set; }
     
        public int PlannedDurationMinutes { get; set; }
      

      
      
    }
}
