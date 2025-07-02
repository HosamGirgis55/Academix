using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Academix.Domain.Entities
{
    public class Student : BaseEntity
    {

        public string? Bio { get; set; } = string.Empty;
        public string? Github { get; set; } = string.Empty;
        public StudentSkiles? studentSkiles { get; set; }
        public Guid? Level { get; set; }
        [ForeignKey("Level")]
        public level? LevelNavigation { get; set; }
        public bool ConnectPrograming { get; set; } = false;

        // Navigation property to ApplicationUser
        public Guid? GraduationStatus { get; set; }
        [ForeignKey("GraduationStatus")]
        public GraduationStatus? GraduationStatusNavigation { get; set; }
        public Guid? SpecialistId { get; set; }
        [ForeignKey("SpecialistId")]
        public Specialist? SpecialistNavigation { get; set; }
        public List<ProblemSolving>? ProblemSolveing { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

    }
    public class ProblemSolvingStudent : BaseEntity
    {
        public Guid ProblemSolvingId { get; set; }
        [ForeignKey("ProblemSolvingId")]
        public ProblemSolving ProblemSolving { get; set; }
        public Guid StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }
    }
    public class ProblemSolving : BaseEntity
    {
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
        
        public Guid StudentId { get; set; }
        [ForeignKey("StudentId")]
        public List<Student>? Student { get; set; }
    }
    public class StudentSkiles : BaseEntity
    {
        public Guid StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }
        public Guid SkilleId { get; set; }
        [ForeignKey("SkilleId")]
        public Skille Skille { get; set; }
    }
    public class Skille: BaseEntity
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public ICollection<Student> Students { get; set; }
    }
    public class  level:BaseEntity
    {
       public string NameAr { get; set; } 
       public string NameEn { get; set; } 
    }
    public class Specialist :BaseEntity
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }

        public ICollection<Student> students { get; set; }
    }
    public class GraduationStatus : BaseEntity
    {
        public StudentSkiles? studentSkiles { get; set; }

        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public ICollection<Student> students { get; set; }
    }


} 