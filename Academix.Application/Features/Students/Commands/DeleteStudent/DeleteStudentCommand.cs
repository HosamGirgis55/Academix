using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;

namespace Academix.Application.Features.Students.Commands.DeleteStudent
{
    public record DeleteStudentCommand(Guid Id) : ICommand<Result>;
} 