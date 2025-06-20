using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;

namespace Academix.Application.Features.Students.Commands.UpdateStudent
{
    public record UpdateStudentCommand : ICommand<Result<bool>>
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string FirstNameAr { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string LastNameAr { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public DateTime DateOfBirth { get; init; }
    }
} 