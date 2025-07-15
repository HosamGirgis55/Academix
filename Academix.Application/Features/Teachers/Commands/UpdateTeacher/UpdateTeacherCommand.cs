using Academix.Application.Common.Models;
using Academix.Domain.Enums;
using MediatR;

namespace Academix.Application.Features.Teachers.Commands.UpdateTeacher
{
    public class UpdateTeacherCommand : IRequest<Result>
    {
        // User identification (will be set from JWT token in endpoint)
        public string UserId { get; set; } = string.Empty;
        
        // User Info - nullable for partial updates
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public Guid? CountryId { get; set; }
        public Gender? Gender { get; set; }

        // Teacher-specific Info - nullable for partial updates
        public string? Bio { get; set; }
        public decimal? Salary { get; set; }
        public List<string>? AdditionalInterests { get; set; }

        // Education and Certificates - nullable for partial updates
        public List<UpdateTeacherEducationDto>? Educations { get; set; }
        public List<UpdateCertificateDto>? Certificates { get; set; }

        // Skills - nullable for partial updates
        public List<UpdateTeacherSkillDto>? Skills { get; set; }
        
        // Teaching Preferences - nullable for partial updates
        public List<Guid>? TeachingAreaIds { get; set; }
        public List<Guid>? AgeGroupIds { get; set; }
        public List<Guid>? CommunicationMethodIds { get; set; }
        public List<Guid>? TeachingLanguageIds { get; set; }
    }
} 