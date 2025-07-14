using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using MediatR;

namespace Academix.Application.Features.Dashboard.Queries.GetSkillById;

public class GetSkillByIdQuery : IRequest<Result<SkillDto>>
{
    public Guid Id { get; set; }
} 