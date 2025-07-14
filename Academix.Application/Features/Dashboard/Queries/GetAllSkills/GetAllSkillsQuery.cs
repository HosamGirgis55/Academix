using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using MediatR;

namespace Academix.Application.Features.Dashboard.Queries.GetAllSkills;

public class GetAllSkillsQuery : IRequest<Result<List<SkillDto>>>
{
} 