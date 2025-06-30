using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using MediatR;

namespace Academix.Application.Features.Common.Queries.GetTimeZones;

public class GetTimeZonesQuery : IQuery<List<TimeZoneDto>>, IRequest<Result<List<TimeZoneDto>>>
{
}

public class TimeZoneDto
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string StandardName { get; set; } = string.Empty;
    public string BaseUtcOffset { get; set; } = string.Empty;
    public bool SupportsDaylightSavingTime { get; set; }
} 