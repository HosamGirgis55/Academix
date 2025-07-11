using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using Academix.Domain.Entities;
using Academix.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;

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
        public List<ExperienceDTO> Experiences { get; set; } = new();
        public List<StudentSkillDto> Skills { get; set; } = new();
        public List<LearningInterestsDto> LearningInterests { get; set; } = new();
    }

     
    public class LearningInterestsDto
    {
      public  Guid LearningInterestId { get; set; }
    }
    public class StudentSkillDto
    {
        public Guid SkillId { get; set; }
     }

    public class ExperienceDTO
    {
        public Guid Id { get; set; }

    }
    public class StudentSkillDTO
    {
        public Guid SkillId { get; set; }
        
    }

    public class StudentRegistrationResponse
    {
        public string UserId { get; set; } = string.Empty;
        public Guid StudentId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
         
        public bool RequiresEmailVerification { get; set; }
      
        public StudentPreferencesDto? Preferences { get; set; }
    }
} 