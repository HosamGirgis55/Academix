using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using MediatR;

namespace Academix.Application.Features.Dashboard.Queries.GetAllSpecializations;

public class GetAllSpecializationsQuery : IRequest<Result<List<SpecializationDto>>>
{
} 