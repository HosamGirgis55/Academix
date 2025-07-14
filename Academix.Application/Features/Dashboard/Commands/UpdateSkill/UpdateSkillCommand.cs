using Academix.Application.Common.Models;
using MediatR;

namespace Academix.Application.Features.Dashboard.Commands.UpdateSkill;

public class UpdateSkillCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
} 