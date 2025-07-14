namespace Academix.Application.Common.Interfaces
{
    public interface IFirebaseNotificationService
    {
        Task<bool> SendSessionRequestNotificationAsync(string deviceToken, string studentName, string subject, Guid sessionRequestId);
        Task<bool> SendSessionAcceptedNotificationAsync(string deviceToken, string teacherName, DateTime scheduledTime);
        Task<bool> SendSessionRejectedNotificationAsync(string deviceToken, string teacherName, string reason);
        Task<bool> SendSessionStartedNotificationAsync(string deviceToken, string participantName);
        Task<bool> SendSessionEndedNotificationAsync(string deviceToken, string participantName);
        Task<bool> SendCustomNotificationAsync(string deviceToken, string title, string body, Dictionary<string, string>? data = null);
    }
} 