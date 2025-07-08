using System;
using System.Collections.Generic;
using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using Academix.Domain.Enums;
using MediatR;

namespace Academix.Application.Features.Students.Commands.RegisterStudent
{
    public class RegisterStudentCommand : IRequest<Result>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public Guid NationalityId { get; set; }
        public int Gender { get; set; }
        public Guid ResidenceCountryId { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public string? Github { get; set; }
        public bool ConnectProgramming { get; set; }
        public Guid LevelId { get; set; }
        public Guid GraduationStatusId { get; set; }
        public Guid SpecialistId { get; set; }
        public List<ExperiencePlatform> Experiences { get; set; } = new();
        public List<StudentSkillDto> Skills { get; set; } = new();
        public List<LearningInterestsDto> LearningInterests { get; set; } = new();
    }

    public class ExperiencePlatform
    {
        public Guid Id { get; set; }
        public string? ProfileUrl { get; set; }
        public int? SolvedProblems { get; set; }
        public int? Rating { get; set; }
    }
    public class LearningInterestsDto
    {
      public  Guid LearningInterestId { get; set; }
    }
    public class StudentSkillDto
    {
        public Guid SkillId { get; set; }
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
        public bool RequiresEmailVerification { get; set; }
        public List<CertificateResponseDto> Certificates { get; set; } = new();
        public List<EducationResponseDto> Educations { get; set; } = new();
        public StudentPreferencesDto? Preferences { get; set; }
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