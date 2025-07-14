using Academix.Application.Common.Models;
using MediatR;

namespace Academix.Application.Features.Dashboard.Commands.DeleteSpecialization;

public class DeleteSpecializationCommand : IRequest<Result>
{
    public Guid Id { get; set; }
} 