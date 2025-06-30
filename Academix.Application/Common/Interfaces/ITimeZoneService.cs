namespace Academix.Application.Common.Interfaces;

public interface ITimeZoneService
{
    /// <summary>
    /// Gets the current user's time zone
    /// </summary>
    /// <returns>Time zone ID (e.g., "UTC", "America/New_York")</returns>
    string GetCurrentTimeZone();
    
    /// <summary>
    /// Sets the current user's time zone
    /// </summary>
    /// <param name="timeZoneId">Time zone ID</param>
    void SetCurrentTimeZone(string timeZoneId);
    
    /// <summary>
    /// Converts UTC time to user's local time zone
    /// </summary>
    /// <param name="utcDateTime">UTC DateTime</param>
    /// <returns>DateTime in user's time zone</returns>
    DateTime ConvertFromUtc(DateTime utcDateTime);
    
    /// <summary>
    /// Converts user's local time to UTC
    /// </summary>
    /// <param name="localDateTime">Local DateTime</param>
    /// <returns>UTC DateTime</returns>
    DateTime ConvertToUtc(DateTime localDateTime);
    
    /// <summary>
    /// Converts UTC time to specified time zone
    /// </summary>
    /// <param name="utcDateTime">UTC DateTime</param>
    /// <param name="timeZoneId">Target time zone ID</param>
    /// <returns>DateTime in specified time zone</returns>
    DateTime ConvertFromUtc(DateTime utcDateTime, string timeZoneId);
    
    /// <summary>
    /// Converts time from specified time zone to UTC
    /// </summary>
    /// <param name="localDateTime">Local DateTime</param>
    /// <param name="timeZoneId">Source time zone ID</param>
    /// <returns>UTC DateTime</returns>
    DateTime ConvertToUtc(DateTime localDateTime, string timeZoneId);
    
    /// <summary>
    /// Gets a list of available time zones
    /// </summary>
    /// <returns>List of time zone information</returns>
    IEnumerable<TimeZoneInfo> GetAvailableTimeZones();
} 