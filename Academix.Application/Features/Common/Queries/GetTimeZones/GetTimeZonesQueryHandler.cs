using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using MediatR;

namespace Academix.Application.Features.Common.Queries.GetTimeZones;

public class GetTimeZonesQueryHandler : IRequestHandler<GetTimeZonesQuery, Result<List<TimeZoneDto>>>
{
    private readonly ITimeZoneService _timeZoneService;
    private readonly ILocalizationService _localizationService;

    public GetTimeZonesQueryHandler(
        ITimeZoneService timeZoneService,
        ILocalizationService localizationService)
    {
        _timeZoneService = timeZoneService;
        _localizationService = localizationService;
    }

    public async Task<Result<List<TimeZoneDto>>> Handle(GetTimeZonesQuery request, CancellationToken cancellationToken)
    {
        var timeZones = _timeZoneService.GetAvailableTimeZones()
            .Select(tz => new TimeZoneDto
            {
                Id = tz.Id,
                DisplayName = tz.DisplayName,
                StandardName = tz.StandardName,
                BaseUtcOffset = tz.BaseUtcOffset.ToString(@"hh\:mm"),
                SupportsDaylightSavingTime = tz.SupportsDaylightSavingTime
            })
            .ToList();

        return await Task.FromResult(Result<List<TimeZoneDto>>.Success(timeZones, _localizationService.GetLocalizedString("OperationCompleted")));
    }
} 