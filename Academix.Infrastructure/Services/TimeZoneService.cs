using Academix.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Academix.Infrastructure.Services;

public class TimeZoneService : ITimeZoneService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string DefaultTimeZone = "UTC";
    private const string TimeZoneHeaderKey = "X-TimeZone";
    private const string TimeZoneSessionKey = "UserTimeZone";

    public TimeZoneService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetCurrentTimeZone()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return DefaultTimeZone;

        // Try to get from session first
        if (httpContext.Session.TryGetValue(TimeZoneSessionKey, out var sessionValue))
        {
            var sessionTimeZone = System.Text.Encoding.UTF8.GetString(sessionValue);
            if (!string.IsNullOrEmpty(sessionTimeZone))
                return sessionTimeZone;
        }

        // Try to get from headers
        if (httpContext.Request.Headers.TryGetValue(TimeZoneHeaderKey, out var headerValue))
        {
            var timeZoneId = headerValue.FirstOrDefault();
            if (!string.IsNullOrEmpty(timeZoneId) && IsValidTimeZone(timeZoneId))
            {
                // Cache in session
                SetCurrentTimeZone(timeZoneId);
                return timeZoneId;
            }
        }

        // Try to get from user claims
        var userTimeZoneClaim = httpContext.User?.FindFirst("timezone")?.Value;
        if (!string.IsNullOrEmpty(userTimeZoneClaim) && IsValidTimeZone(userTimeZoneClaim))
        {
            return userTimeZoneClaim;
        }

        return DefaultTimeZone;
    }

    public void SetCurrentTimeZone(string timeZoneId)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null || !IsValidTimeZone(timeZoneId))
            return;

        var timeZoneBytes = System.Text.Encoding.UTF8.GetBytes(timeZoneId);
        httpContext.Session.Set(TimeZoneSessionKey, timeZoneBytes);
    }

    public DateTime ConvertFromUtc(DateTime utcDateTime)
    {
        return ConvertFromUtc(utcDateTime, GetCurrentTimeZone());
    }

    public DateTime ConvertToUtc(DateTime localDateTime)
    {
        return ConvertToUtc(localDateTime, GetCurrentTimeZone());
    }

    public DateTime ConvertFromUtc(DateTime utcDateTime, string timeZoneId)
    {
        if (!IsValidTimeZone(timeZoneId))
            return utcDateTime;

        try
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
        }
        catch
        {
            return utcDateTime;
        }
    }

    public DateTime ConvertToUtc(DateTime localDateTime, string timeZoneId)
    {
        if (!IsValidTimeZone(timeZoneId))
            return localDateTime;

        try
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeToUtc(localDateTime, timeZone);
        }
        catch
        {
            return localDateTime;
        }
    }

    public IEnumerable<TimeZoneInfo> GetAvailableTimeZones()
    {
        return TimeZoneInfo.GetSystemTimeZones().OrderBy(tz => tz.DisplayName);
    }

    private static bool IsValidTimeZone(string timeZoneId)
    {
        if (string.IsNullOrEmpty(timeZoneId))
            return false;

        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return true;
        }
        catch
        {
            return false;
        }
    }
} 