using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using MediatR;

namespace Academix.Application.Features.Common.Queries.GetLookup;

public class GetLookupQuery :  IRequest<Result<List<LookupItemDto>>>
{
    public string Type { get; set; } = string.Empty;
} 