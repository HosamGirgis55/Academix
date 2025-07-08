using Academix.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;

namespace Academix.Application.Features.Teachers.Commands.RegisterTeacher
{
    public class RegisterTeacherCommand : IRequest<Result>
    {
        // User Info
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int Gender { get; set; }

        // Basic Info
        public string Bio { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public List<string> AdditionalInterests { get; set; } = new();

        // Location Info
        public Guid NationalityId { get; set; }
        public Guid CountryId { get; set; }

        // Education
        public List<TeacherEducationDto> Educations { get; set; } = new();
        public List<TeacherCertificateDto> Certificates { get; set; } = new();

        // Teaching Preferences
        public List<Guid> TeachingAreaIds { get; set; } = new();
        public List<Guid> AgeGroupIds { get; set; } = new();
        public List<Guid> CommunicationMethodIds { get; set; } = new();
        public List<Guid> TeachingLanguageIds { get; set; } = new();
    }

    public class TeacherEducationDto
    {
        public string Institution { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string Field { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class TeacherCertificateDto
    {
        public string Name { get; set; } = string.Empty;
        public string CertificateUrl { get; set; } = string.Empty;
        public string IssuedBy { get; set; } = string.Empty;
        public DateTime IssuedDate { get; set; }
        public string ExamResult { get; set; } = string.Empty;
    }
} 