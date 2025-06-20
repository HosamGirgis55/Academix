using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;

namespace Academix.Application.Features.Students.Queries.GetStudentById
{
    public record GetStudentByIdQuery(Guid StudentId) : IQuery<Result<StudentDto>>;

    public class StudentDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string StudentNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 