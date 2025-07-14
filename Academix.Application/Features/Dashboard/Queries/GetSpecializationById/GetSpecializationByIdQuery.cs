using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using MediatR;

namespace Academix.Application.Features.Dashboard.Queries.GetSpecializationById;

public class GetSpecializationByIdQuery : IRequest<Result<SpecializationDto>>
{
    public Guid Id { get; set; }
} 