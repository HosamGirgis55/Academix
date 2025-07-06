using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Academix.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly IMemoryCache _cache;

    public EmailService(IOptions<EmailSettings> emailSettings, IMemoryCache cache)
    {
        _emailSettings = emailSettings.Value;
        _cache = cache;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            using var client = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort);
            client.EnableSsl = _emailSettings.EnableSsl;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);

            using var message = new MailMessage();
            message.From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName);
            message.To.Add(to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isHtml;

            await client.SendMailAsync(message);
            return true;
        }
        catch (Exception)
        {
            // Log exception here if logging is available
            return false;
        }
    }

    public async Task<bool> SendOtpEmailAsync(string to, string otp, string purpose)
    {
        var subject = purpose switch
        {
            "registration" => "Confirm Your Email - Academix",
            "password-reset" => "Password Reset Code - Academix",
            _ => "Verification Code - Academix"
        };

        var body = purpose switch
        {
            "registration" => GenerateRegistrationEmailBody(otp),
            "password-reset" => GeneratePasswordResetEmailBody(otp),
            _ => GenerateGenericOtpEmailBody(otp, purpose)
        };

        return await SendEmailAsync(to, subject, body);
    }

    public async Task<string> GenerateOtpAsync(string email, string purpose)
    {
        var otp = GenerateRandomOtp();
        var cacheKey = $"otp_{email}_{purpose}";
        var expiration = TimeSpan.FromMinutes(_emailSettings.OtpExpirationMinutes);
        
        _cache.Set(cacheKey, otp, expiration);
        
        return await Task.FromResult(otp);
    }

    public async Task<bool> ValidateOtpAsync(string email, string otp, string purpose)
    {
        var cacheKey = $"otp_{email}_{purpose}";
        
        if (_cache.TryGetValue(cacheKey, out string? cachedOtp))
        {
            if (cachedOtp == otp)
            {
                _cache.Remove(cacheKey); // Remove OTP after successful validation
                return await Task.FromResult(true);
            }
        }
        
        return await Task.FromResult(false);
    }

    public async Task<bool> SendRegistrationConfirmationAsync(string to, string otp)
    {
        return await SendOtpEmailAsync(to, otp, "registration");
    }

    public async Task<bool> SendPasswordResetOtpAsync(string to, string otp)
    {
        return await SendOtpEmailAsync(to, otp, "password-reset");
    }

    private string GenerateRandomOtp()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        var randomNumber = BitConverter.ToUInt32(bytes, 0);
        var otp = (randomNumber % (uint)Math.Pow(10, _emailSettings.OtpLength)).ToString($"D{_emailSettings.OtpLength}");
        return otp;
    }

    private string GenerateRegistrationEmailBody(string otp)
    {
        return $@"
            <html>
            <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                <div style='background-color: #f8f9fa; padding: 30px; border-radius: 10px; text-align: center;'>
                    <h2 style='color: #007bff; margin-bottom: 20px;'>Welcome to Academix!</h2>
                    <p style='font-size: 16px; color: #333; margin-bottom: 20px;'>
                        Thank you for registering with us. Please use the following verification code to confirm your email address:
                    </p>
                    <div style='background-color: #007bff; color: white; font-size: 24px; font-weight: bold; padding: 15px; border-radius: 5px; margin: 20px 0; letter-spacing: 3px;'>
                        {otp}
                    </div>
                    <p style='font-size: 14px; color: #666; margin-top: 20px;'>
                        This code will expire in {_emailSettings.OtpExpirationMinutes} minutes.
                    </p>
                    <p style='font-size: 14px; color: #666;'>
                        If you didn't request this verification, please ignore this email.
                    </p>
                </div>
            </body>
            </html>";
    }

    private string GeneratePasswordResetEmailBody(string otp)
    {
        return $@"
            <html>
            <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                <div style='background-color: #f8f9fa; padding: 30px; border-radius: 10px; text-align: center;'>
                    <h2 style='color: #dc3545; margin-bottom: 20px;'>Password Reset Request</h2>
                    <p style='font-size: 16px; color: #333; margin-bottom: 20px;'>
                        We received a request to reset your password. Please use the following verification code:
                    </p>
                    <div style='background-color: #dc3545; color: white; font-size: 24px; font-weight: bold; padding: 15px; border-radius: 5px; margin: 20px 0; letter-spacing: 3px;'>
                        {otp}
                    </div>
                    <p style='font-size: 14px; color: #666; margin-top: 20px;'>
                        This code will expire in {_emailSettings.OtpExpirationMinutes} minutes.
                    </p>
                    <p style='font-size: 14px; color: #666;'>
                        If you didn't request a password reset, please ignore this email and your password will remain unchanged.
                    </p>
                </div>
            </body>
            </html>";
    }

    private string GenerateGenericOtpEmailBody(string otp, string purpose)
    {
        return $@"
            <html>
            <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                <div style='background-color: #f8f9fa; padding: 30px; border-radius: 10px; text-align: center;'>
                    <h2 style='color: #007bff; margin-bottom: 20px;'>Verification Code</h2>
                    <p style='font-size: 16px; color: #333; margin-bottom: 20px;'>
                        Your verification code for {purpose} is:
                    </p>
                    <div style='background-color: #007bff; color: white; font-size: 24px; font-weight: bold; padding: 15px; border-radius: 5px; margin: 20px 0; letter-spacing: 3px;'>
                        {otp}
                    </div>
                    <p style='font-size: 14px; color: #666; margin-top: 20px;'>
                        This code will expire in {_emailSettings.OtpExpirationMinutes} minutes.
                    </p>
                </div>
            </body>
            </html>";
    }
} 