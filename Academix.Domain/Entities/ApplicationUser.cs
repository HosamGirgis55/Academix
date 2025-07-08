using Academix.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }

        public Gender Gender { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Guid? CountryId { get; set; }
        [ForeignKey(nameof(CountryId))]
        public Country? Country { get; set; } = default!;

        // Authentication properties
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpiry { get; set; }
        
        // Email Verification OTP
        public string? EmailVerificationOtp { get; set; }
        public DateTime? EmailVerificationOtpExpiry { get; set; }
        public string? PasswordResetOtp { get; set; }
        public DateTime? PasswordResetOtpExpiry { get; set; }
        
        // Time Zone Configuration
        public string TimeZone { get; set; } = "UTC";

        // Navigation property to National
        public Guid? NationalityId { get; set; }
        [ForeignKey(nameof(NationalityId))]
        public Nationality? Nationality { get; set; }


        public Student? Student { get; set; }

        public Teacher? Teacher { get; set; }
    }

}
