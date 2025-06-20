namespace Academix.Domain.Entities
{
    public class Student : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string FirstNameAr { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string LastNameAr { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string StudentNumber { get; set; } = string.Empty;
        
        // Navigation properties can be added later when implementing course enrollment
        // public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        
        // Helper methods for localization
        public string GetFirstName(string culture) => culture == "ar" ? FirstNameAr : FirstName;
        public string GetLastName(string culture) => culture == "ar" ? LastNameAr : LastName;
        public string GetFullName(string culture) => $"{GetFirstName(culture)} {GetLastName(culture)}";
    }
} 