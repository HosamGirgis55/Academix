using Microsoft.AspNetCore.Identity;
using Academix.Domain.Enums;

namespace Academix.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? TimeZone { get; set; }
        public string? EmailOtp { get; set; }
        public DateTime? EmailOtpExpiry { get; set; }
        public string? PasswordResetOtp { get; set; }
        public DateTime? PasswordResetOtpExpiry { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }  // Changed from RefreshTokenExpiry
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpiry { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public Gender Gender { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CountryId { get; set; }
        public Country? Country { get; set; }
        
        // Firebase Notification
        public string? DeviceToken { get; set; }
        
        // Points system for all users
        public int Points { get; set; } = 0;
    }
}
