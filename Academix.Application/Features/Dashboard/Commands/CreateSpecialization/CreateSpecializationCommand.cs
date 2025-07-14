using Academix.Application.Common.Models;
using MediatR;

namespace Academix.Application.Features.Dashboard.Commands.CreateSpecialization;

public class CreateSpecializationCommand : IRequest<Result>
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
} 