using Academix.Application.Common.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Academix.Infrastructure.Services
{
    public class FirebaseNotificationService : IFirebaseNotificationService
    {
        private readonly ILogger<FirebaseNotificationService> _logger;
        private readonly IConfiguration _configuration;
        private readonly FirebaseMessaging _messaging;

        public FirebaseNotificationService(
            ILogger<FirebaseNotificationService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            // Initialize Firebase if not already initialized
            if (FirebaseApp.DefaultInstance == null)
            {
                try
                {
                    var firebaseSection = _configuration.GetSection("Firebase");
                    if (firebaseSection.Exists())
                    {
                        // Build the Firebase credentials JSON manually
                        var firebaseCredentials = new
                        {
                            type = firebaseSection["type"],
                            project_id = firebaseSection["project_id"],
                            private_key_id = firebaseSection["private_key_id"],
                            private_key = firebaseSection["private_key"],
                            client_email = firebaseSection["client_email"],
                            client_id = firebaseSection["client_id"],
                            auth_uri = firebaseSection["auth_uri"],
                            token_uri = firebaseSection["token_uri"],
                            auth_provider_x509_cert_url = firebaseSection["auth_provider_x509_cert_url"],
                            client_x509_cert_url = firebaseSection["client_x509_cert_url"],
                            universe_domain = firebaseSection["universe_domain"]
                        };

                        var credentialJson = JsonSerializer.Serialize(firebaseCredentials);
                        var credential = GoogleCredential.FromJson(credentialJson);
                        
                        FirebaseApp.Create(new AppOptions()
                        {
                            Credential = credential
                        });
                        
                        _logger.LogInformation("Firebase initialized successfully.");
                    }
                    else
                    {
                        _logger.LogWarning("Firebase configuration not found. Push notifications will not work.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize Firebase. Push notifications will not work.");
                }
            }

            _messaging = FirebaseMessaging.DefaultInstance;
        }

        public async Task<bool> SendSessionRequestNotificationAsync(string deviceToken, string studentName, string subject, Guid sessionRequestId)
        {
            var title = "New Session Request";
            var body = $"{studentName} is requesting a session for {subject}";
            var data = new Dictionary<string, string>
            {
                ["type"] = "session_request",
                ["sessionRequestId"] = sessionRequestId.ToString(),
                ["studentName"] = studentName,
                ["subject"] = subject
            };

            return await SendNotificationAsync(deviceToken, title, body, data);
        }

        public async Task<bool> SendSessionAcceptedNotificationAsync(string deviceToken, string teacherName, DateTime scheduledTime)
        {
            var title = "Session Accepted";
            var body = $"{teacherName} accepted your session request. Scheduled for {scheduledTime:MMM dd, yyyy 'at' HH:mm}";
            var data = new Dictionary<string, string>
            {
                ["type"] = "session_accepted",
                ["teacherName"] = teacherName,
                ["scheduledTime"] = scheduledTime.ToString("O")
            };

            return await SendNotificationAsync(deviceToken, title, body, data);
        }

        public async Task<bool> SendSessionRejectedNotificationAsync(string deviceToken, string teacherName, string reason)
        {
            var title = "Session Request Rejected";
            var body = $"{teacherName} declined your session request. Reason: {reason}";
            var data = new Dictionary<string, string>
            {
                ["type"] = "session_rejected",
                ["teacherName"] = teacherName,
                ["reason"] = reason
            };

            return await SendNotificationAsync(deviceToken, title, body, data);
        }

        public async Task<bool> SendSessionStartedNotificationAsync(string deviceToken, string participantName)
        {
            var title = "Session Started";
            var body = $"Your session with {participantName} has started";
            var data = new Dictionary<string, string>
            {
                ["type"] = "session_started",
                ["participantName"] = participantName
            };

            return await SendNotificationAsync(deviceToken, title, body, data);
        }

        public async Task<bool> SendSessionEndedNotificationAsync(string deviceToken, string participantName)
        {
            var title = "Session Ended";
            var body = $"Your session with {participantName} has ended";
            var data = new Dictionary<string, string>
            {
                ["type"] = "session_ended",
                ["participantName"] = participantName
            };

            return await SendNotificationAsync(deviceToken, title, body, data);
        }

        public async Task<bool> SendCustomNotificationAsync(string deviceToken, string title, string body, Dictionary<string, string>? data = null)
        {
            return await SendNotificationAsync(deviceToken, title, body, data);
        }

        private async Task<bool> SendNotificationAsync(string deviceToken, string title, string body, Dictionary<string, string>? data = null)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceToken))
                {
                    _logger.LogWarning("Device token is empty. Cannot send notification.");
                    return false;
                }

                var message = new Message()
                {
                    Token = deviceToken,
                    Notification = new Notification()
                    {
                        Title = title,
                        Body = body
                    },
                    Data = data,
                    Android = new AndroidConfig()
                    {
                        Notification = new AndroidNotification()
                        {
                            Icon = "ic_notification",
                            Color = "#4285F4",
                            Sound = "default"
                        }
                    },
                    Apns = new ApnsConfig()
                    {
                        Aps = new Aps()
                        {
                            Alert = new ApsAlert()
                            {
                                Title = title,
                                Body = body
                            },
                            Sound = "default"
                        }
                    }
                };

                var response = await _messaging.SendAsync(message);
                _logger.LogInformation($"Successfully sent message: {response}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send Firebase notification to token: {DeviceToken}", deviceToken);
                return false;
            }
        }
    }
} 