using Academix.Application.Common.Interfaces;

namespace Academix.Application.Common.Extensions;

public static class TimeZoneExtensions
{
    /// <summary>
    /// Converts DateTime to user's local time zone
    /// </summary>
    /// <param name="dateTime">UTC DateTime</param>
    /// <param name="timeZoneService">Time zone service</param>
    /// <returns>DateTime in user's time zone</returns>
    public static DateTime ToUserTimeZone(this DateTime dateTime, ITimeZoneService timeZoneService)
    {
        if (dateTime.Kind == DateTimeKind.Utc)
        {
            return timeZoneService.ConvertFromUtc(dateTime);
        }
        
        // If it's not UTC, assume it's already in local time or convert to UTC first
        return dateTime.Kind == DateTimeKind.Local 
            ? timeZoneService.ConvertFromUtc(dateTime.ToUniversalTime()) 
            : timeZoneService.ConvertFromUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));
    }

    /// <summary>
    /// Converts DateTime to user's local time zone (nullable version)
    /// </summary>
    /// <param name="dateTime">UTC DateTime (nullable)</param>
    /// <param name="timeZoneService">Time zone service</param>
    /// <returns>DateTime in user's time zone or null</returns>
    public static DateTime? ToUserTimeZone(this DateTime? dateTime, ITimeZoneService timeZoneService)
    {
        return dateTime?.ToUserTimeZone(timeZoneService);
    }

    /// <summary>
    /// Converts DateTime from user's time zone to UTC
    /// </summary>
    /// <param name="dateTime">DateTime in user's time zone</param>
    /// <param name="timeZoneService">Time zone service</param>
    /// <returns>UTC DateTime</returns>
    public static DateTime ToUtc(this DateTime dateTime, ITimeZoneService timeZoneService)
    {
        if (dateTime.Kind == DateTimeKind.Utc)
        {
            return dateTime;
        }
        
        return timeZoneService.ConvertToUtc(dateTime);
    }

    /// <summary>
    /// Converts DateTime from user's time zone to UTC (nullable version)
    /// </summary>
    /// <param name="dateTime">DateTime in user's time zone (nullable)</param>
    /// <param name="timeZoneService">Time zone service</param>
    /// <returns>UTC DateTime or null</returns>
    public static DateTime? ToUtc(this DateTime? dateTime, ITimeZoneService timeZoneService)
    {
        return dateTime?.ToUtc(timeZoneService);
    }

    /// <summary>
    /// Formats DateTime with time zone information
    /// </summary>
    /// <param name="dateTime">DateTime to format</param>
    /// <param name="timeZoneService">Time zone service</param>
    /// <param name="format">Format string (optional)</param>
    /// <returns>Formatted date string with time zone info</returns>
    public static string ToUserTimeZoneString(this DateTime dateTime, ITimeZoneService timeZoneService, string format = "yyyy-MM-dd HH:mm:ss")
    {
        var userDateTime = dateTime.ToUserTimeZone(timeZoneService);
        var timeZoneId = timeZoneService.GetCurrentTimeZone();
        
        return $"{userDateTime.ToString(format)} ({timeZoneId})";
    }

    /// <summary>
    /// Formats DateTime with time zone information (nullable version)
    /// </summary>
    /// <param name="dateTime">DateTime to format (nullable)</param>
    /// <param name="timeZoneService">Time zone service</param>
    /// <param name="format">Format string (optional)</param>
    /// <returns>Formatted date string with time zone info or empty string</returns>
    public static string ToUserTimeZoneString(this DateTime? dateTime, ITimeZoneService timeZoneService, string format = "yyyy-MM-dd HH:mm:ss")
    {
        return dateTime?.ToUserTimeZoneString(timeZoneService, format) ?? string.Empty;
    }
} 