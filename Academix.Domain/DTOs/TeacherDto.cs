using Academix.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.DTOs
{
    public class TeacherDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public double salary { get; set; }
        public List<TeacherSkillDto> Skills { get; set; } = new();
        public List<TeacherCommentDto> Comments { get; set; } = new();
        public TeacherRatingInfoDto Rating { get; set; } = new();
    }

    public class TeacherSkillDto
    {
        public Guid SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public int Level { get; set; }
    }

    public class TeacherCommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public TeacherStudentInfoDto Student { get; set; } = null!;
    }

    public class TeacherStudentInfoDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }

    public class TeacherRatingInfoDto
    {
        public double AverageRating { get; set; }
        public int TotalComments { get; set; }
    }
}
