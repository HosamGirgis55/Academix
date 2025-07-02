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
        public Guid? NatinalityId { get; set; }
        public Gender Gender { get; set; }
        public Guid CountryId { get; set; }
        public string? ProfilePictureUrl { get; set; }

        public string? Bio { get; set; } = string.Empty;
        public string? Github { get; set; } = string.Empty;
        public bool ConnectPrograming { get; set; } = false;

       
        // Level
        public Guid? LevelId { get; set; }

        // Graduation Status
        public Guid? GraduationStatusId { get; set; }


        // Specialist
        public Guid? SpecialistId { get; set; }
     

        // Problem solving tags or keywords
        public List<ProblemSolveingDTO>? ProblemSolveing { get; set; }

        // Skills
        public List<StudentSkillDTO>? Skills { get; set; }

    }

    public class ProblemSolveingDTO
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
      
    }

     

     
} 