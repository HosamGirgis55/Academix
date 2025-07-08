using Academix.Application.Common.Interfaces;
using Academix.Domain.DTOs;

namespace Academix.Application.Features.Lookups.Queries.GetLookup;

public class GetLookupQuery : IQuery<List<LookupItemDto>>
{
    public string Type { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
} 