using Academix.Application.Common.Models;
using MediatR;

namespace Academix.Application.Features.Dashboard.Commands.DeleteSkill;

public class DeleteSkillCommand : IRequest<Result>
{
    public Guid Id { get; set; }
} 