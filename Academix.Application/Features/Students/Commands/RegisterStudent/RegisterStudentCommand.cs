using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Enums;

namespace Academix.Application.Features.Students.Commands.RegisterStudent
{
    public class RegisterStudentCommand : ICommand<Result<StudentRegistrationResponse>>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public Gender Gender { get; set; }
        public Guid CountryId { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public List<CreateCertificateDto>? Certificates { get; set; }
        public List<CreateEducationDto>? Educations { get; set; }
     }

    public class CreateCertificateDto
    {
        public string Name { get; set; } = string.Empty;
        public string CertificateUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime IssuedDate { get; set; }
        public string IssuedBy { get; set; } = string.Empty;
    }

    public class CreateEducationDto
    {
        public string Degree { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FieldOfStudy { get; set; } = string.Empty;
    }

    public class StudentRegistrationResponse
    {
        public string UserId { get; set; } = string.Empty;
        public Guid StudentId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int CertificatesCount { get; set; }
        public int EducationsCount { get; set; }
        public List<CertificateResponseDto> Certificates { get; set; } = new();
        public List<EducationResponseDto> Educations { get; set; } = new();
    }

    public class CertificateResponseDto
    {
        public string Name { get; set; } = string.Empty;
        public string CertificateUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime IssuedDate { get; set; }
        public string IssuedBy { get; set; } = string.Empty;
    }

    public class EducationResponseDto
    {
        public string Degree { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FieldOfStudy { get; set; } = string.Empty;
    }
} 