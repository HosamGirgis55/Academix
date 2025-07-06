using Academix.Application.Common.Interfaces;

namespace Academix.WebAPI.Common.Middleware;

public class TimeZoneMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TimeZoneMiddleware> _logger;

    public TimeZoneMiddleware(RequestDelegate next, ILogger<TimeZoneMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITimeZoneService timeZoneService)
    {
        try
        {
            // Extract time zone from various sources
            await ProcessTimeZoneHeaders(context, timeZoneService);
            
            // Continue with the request
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in TimeZone middleware");
            // Continue with the request even if time zone processing fails
            await _next(context);
        }
    }

    private static async Task ProcessTimeZoneHeaders(HttpContext context, ITimeZoneService timeZoneService)
    {
        // Try to get time zone from headers
        if (context.Request.Headers.TryGetValue("X-TimeZone", out var timeZoneHeader))
        {
            var timeZoneId = timeZoneHeader.FirstOrDefault();
            if (!string.IsNullOrEmpty(timeZoneId))
            {
                timeZoneService.SetCurrentTimeZone(timeZoneId);
            }
        }

        // Try to get time zone offset from headers (for browsers that send offset)
        if (context.Request.Headers.TryGetValue("X-TimeZone-Offset", out var offsetHeader))
        {
            if (int.TryParse(offsetHeader.FirstOrDefault(), out var offsetMinutes))
            {
                // Convert offset to time zone (this is a simplified approach)
                var timeZoneId = ConvertOffsetToTimeZoneId(offsetMinutes);
                if (!string.IsNullOrEmpty(timeZoneId))
                {
                    timeZoneService.SetCurrentTimeZone(timeZoneId);
                }
            }
        }

        await Task.CompletedTask;
    }

    private static string? ConvertOffsetToTimeZoneId(int offsetMinutes)
    {
        // This is a simplified mapping - in production, you might want a more comprehensive mapping
        var offsetHours = offsetMinutes / 60.0;
        
        return offsetHours switch
        {
            0 => "UTC",
            -5 => "America/New_York", // EST
            -6 => "America/Chicago", // CST
            -7 => "America/Denver", // MST
            -8 => "America/Los_Angeles", // PST
            1 => "Europe/London", // GMT+1
            2 => "Europe/Berlin", // GMT+2
            3 => "Europe/Moscow", // GMT+3
            8 => "Asia/Singapore", // GMT+8
            9 => "Asia/Tokyo", // GMT+9
            _ => null
        };
    }
} 