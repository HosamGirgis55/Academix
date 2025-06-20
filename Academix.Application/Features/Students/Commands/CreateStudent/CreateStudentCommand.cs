using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;

namespace Academix.Application.Features.Students.Commands.CreateStudent
{
    public record CreateStudentCommand : ICommand<Result<Guid>>
    {
        public string FirstName { get; init; } = string.Empty;
        public string FirstNameAr { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string LastNameAr { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public DateTime DateOfBirth { get; init; }
        public string StudentNumber { get; init; } = string.Empty;
    }
} 