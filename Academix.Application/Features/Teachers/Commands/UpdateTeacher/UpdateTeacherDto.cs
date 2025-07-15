using System.ComponentModel.DataAnnotations;
using Academix.Domain.Enums;

namespace Academix.Application.Features.Teachers.Commands.UpdateTeacher;

public class UpdateTeacherDto
{
    [StringLength(100, MinimumLength = 2)]
    public string? FirstName { get; set; }
    
    [StringLength(100, MinimumLength = 2)]
    public string? LastName { get; set; }
    
    public string? ProfilePictureUrl { get; set; }
    
    public Guid? CountryId { get; set; }
    
    // Gender using enum directly - accepts both int (0/1) and string ("Male"/"Female") via JSON
    public Gender? Gender { get; set; }
    
    // Teacher-specific fields
    [StringLength(1000)]
    public string? Bio { get; set; }
    
    [Range(0, 10000)]
    public decimal? Salary { get; set; }
    
    public List<string>? AdditionalInterests { get; set; }
    
    // Complex objects - null means no change, empty list means clear all
    public List<UpdateTeacherEducationDto>? Educations { get; set; }
    public List<UpdateCertificateDto>? Certificates { get; set; }
    public List<UpdateTeacherSkillDto>? Skills { get; set; }
    
    // Teaching preferences - null means no change, empty list means clear all
    public List<Guid>? TeachingAreaIds { get; set; }
    public List<Guid>? AgeGroupIds { get; set; }
    public List<Guid>? CommunicationMethodIds { get; set; }
    public List<Guid>? TeachingLanguageIds { get; set; }
}

public class UpdateTeacherEducationDto
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Institution { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Degree { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Field { get; set; } = string.Empty;
    
    [Required]
    public DateTime StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
}

public class UpdateCertificateDto
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    public string CertificateUrl { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string IssuedBy { get; set; } = string.Empty;
    
    [Required]
    public DateTime IssuedDate { get; set; }
    
    [StringLength(100)]
    public string ExamResult { get; set; } = string.Empty;
}

public class UpdateTeacherSkillDto
{
    [Required]
    public Guid SkillId { get; set; }
} 