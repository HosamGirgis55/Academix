using System.Text.Json.Serialization;

namespace Academix.Domain.DTOs;

public class StudentPreferencesDto
{
    [JsonPropertyName("learningInterests")]
    public List<string> LearningInterests { get; set; } = new();

    [JsonPropertyName("preferredLanguages")]
    public List<string> PreferredLanguages { get; set; } = new();

    [JsonPropertyName("studyPreferences")]
    public StudyPreferences StudyPreferences { get; set; } = new();

    [JsonPropertyName("notifications")]
    public NotificationPreferences Notifications { get; set; } = new();
}

public class StudyPreferences
{
    [JsonPropertyName("preferredStudyTime")]
    public string PreferredStudyTime { get; set; } = "morning";

    [JsonPropertyName("preferredStudyDuration")]
    public int PreferredStudyDuration { get; set; } = 60; // in minutes

    [JsonPropertyName("preferredStudyEnvironment")]
    public string PreferredStudyEnvironment { get; set; } = "quiet";
}

public class NotificationPreferences
{
    [JsonPropertyName("email")]
    public bool Email { get; set; } = true;

    [JsonPropertyName("push")]
    public bool Push { get; set; } = true;

    [JsonPropertyName("sms")]
    public bool SMS { get; set; } = false;
} 