using Academix.Application.Common.Models;
using MediatR;

namespace Academix.Application.Features.Teachers.Commands.UpdateTeacher
{
    public class UpdateTeacherCommand : IRequest<Result>
    {
        // User identification (will be set from JWT token in endpoint)
        public string UserId { get; set; } = string.Empty;
        
        // User Info
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public Guid CountryId { get; set; }
        public string Gender { get; set; } = string.Empty;

        // Teacher-specific Info
        public string Bio { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public List<string> AdditionalInterests { get; set; } = new();

        // Education and Certificates
        public List<UpdateTeacherEducationDto> Educations { get; set; } = new();
        public List<UpdateCertificateDto> Certificates { get; set; } = new();

        // Skills
        public List<UpdateTeacherSkillDto> Skills { get; set; } = new();
        
        // Teaching Preferences
        public List<Guid> TeachingAreaIds { get; set; } = new();
        public List<Guid> AgeGroupIds { get; set; } = new();
        public List<Guid> CommunicationMethodIds { get; set; } = new();
        public List<Guid> TeachingLanguageIds { get; set; } = new();
    }
}
