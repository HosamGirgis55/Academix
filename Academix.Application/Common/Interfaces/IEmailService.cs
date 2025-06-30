namespace Academix.Application.Common.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task<bool> SendOtpEmailAsync(string to, string otp, string purpose);
    Task<string> GenerateOtpAsync(string email, string purpose);
    Task<bool> ValidateOtpAsync(string email, string otp, string purpose);
    Task<bool> SendRegistrationConfirmationAsync(string to, string otp);
    Task<bool> SendPasswordResetOtpAsync(string to, string otp);
} 