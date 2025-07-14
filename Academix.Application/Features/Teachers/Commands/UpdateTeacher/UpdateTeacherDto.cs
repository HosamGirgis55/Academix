using System.ComponentModel.DataAnnotations;

namespace Academix.Application.Features.Teachers.Commands.UpdateTeacher;

public class UpdateTeacherDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string LastName { get; set; } = string.Empty;
    
    public string? ProfilePictureUrl { get; set; }
    
    [Required]
    public Guid CountryId { get; set; }
    
    [Required]
    public string Gender { get; set; } = string.Empty;
    
    // Teacher-specific fields
    [StringLength(1000)]
    public string Bio { get; set; } = string.Empty;
    
    [Range(0, 10000)]
    public decimal Salary { get; set; }
    
    public List<string> AdditionalInterests { get; set; } = new();
    
    // Complex objects
    public List<UpdateTeacherEducationDto> Educations { get; set; } = new();
    public List<UpdateCertificateDto> Certificates { get; set; } = new();
    public List<UpdateTeacherSkillDto> Skills { get; set; } = new();
    
    // Teaching preferences
    public List<Guid> TeachingAreaIds { get; set; } = new();
    public List<Guid> AgeGroupIds { get; set; } = new();
    public List<Guid> CommunicationMethodIds { get; set; } = new();
    public List<Guid> TeachingLanguageIds { get; set; } = new();
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