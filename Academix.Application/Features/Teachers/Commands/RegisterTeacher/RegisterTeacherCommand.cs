using Academix.Application.Common.Models;
using Academix.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;

namespace Academix.Application.Features.Teachers.Commands.RegisterTeacher
{
    public class RegisterTeacherCommand : IRequest<Result>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public Guid NationalityId { get; set; }
        public Gender Gender { get; set; }
        public Guid CountryId { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public List<TeacherCertificateDto> Certificates { get; set; } = new();
        public List<TeacherEducationDto> Educations { get; set; } = new();
        public List<TeacherExamsDto> TeacherExams { get; set; } = new();

    }

    public class TeacherCertificateDto
    {
        public string Name { get; set; }
        public string CertificateUrl { get; set; }
        public string Description { get; set; }
        public DateTime IssuedDate { get; set; }
        public string IssuedBy { get; set; }
    }

    public class TeacherEducationDto
    {
        public string Degree { get; set; }
        public string Institution { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FieldOfStudy { get; set; }
    }

    public class TeacherExamsDto
    {
        public string Name { get; set; }
        public string ExamResult { get; set; }
        public string IssuedBy { get; set; }
        public DateTime IssuedDate { get; set; }
        public string ExameCertificateUrl { get; set; }

    }
} 