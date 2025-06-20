using Academix.Application.Features.Students.Commands.CreateStudent;
using Academix.Application.Features.Students.Commands.UpdateStudent;
using System.Text.RegularExpressions;

namespace Academix.Application.Common.Mappings
{
    public static class ValidationExtensions
    {
        #region Email Validation

        /// <summary>
        /// Validates email format using regex
        /// </summary>
        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            const string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailPattern);
        }

        #endregion

        #region Student Number Validation

        /// <summary>
        /// Validates student number format (8 digits)
        /// </summary>
        public static bool IsValidStudentNumber(this string studentNumber)
        {
            if (string.IsNullOrWhiteSpace(studentNumber))
                return false;

            return studentNumber.Length == 8 && studentNumber.All(char.IsDigit);
        }

        #endregion

        #region Name Validation

        /// <summary>
        /// Validates name (Arabic or English characters only)
        /// </summary>
        public static bool IsValidName(this string name, bool isArabic = false)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (name.Length < 2 || name.Length > 50)
                return false;

            if (isArabic)
            {
                // Arabic name validation
                const string arabicPattern = @"^[\u0600-\u06FF\s]+$";
                return Regex.IsMatch(name, arabicPattern);
            }
            else
            {
                // English name validation
                const string englishPattern = @"^[a-zA-Z\s]+$";
                return Regex.IsMatch(name, englishPattern);
            }
        }

        #endregion

        #region Age Validation

        /// <summary>
        /// Validates age is within acceptable range (16-100 years)
        /// </summary>
        public static bool IsValidAge(this DateTime dateOfBirth)
        {
            var age = dateOfBirth.CalculateAge();
            return age >= 16 && age <= 100;
        }

        /// <summary>
        /// Validates date of birth is not in the future
        /// </summary>
        public static bool IsValidDateOfBirth(this DateTime dateOfBirth)
        {
            return dateOfBirth <= DateTime.Today;
        }

        #endregion

        #region Command Validation Extensions

        /// <summary>
        /// Validates CreateStudentCommand
        /// </summary>
        public static (bool IsValid, List<string> Errors) ValidateCommand(this CreateStudentCommand command, string culture = "en")
        {
            var errors = new List<string>();

            // Validate English names
            if (!command.FirstName.IsValidName(false))
                errors.Add(culture == "ar" ? "الاسم الأول باللغة الإنجليزية غير صحيح" : "First name is invalid");

            if (!command.LastName.IsValidName(false))
                errors.Add(culture == "ar" ? "اسم العائلة باللغة الإنجليزية غير صحيح" : "Last name is invalid");

            // Validate Arabic names if provided
            if (!string.IsNullOrWhiteSpace(command.FirstNameAr) && !command.FirstNameAr.IsValidName(true))
                errors.Add(culture == "ar" ? "الاسم الأول باللغة العربية غير صحيح" : "Arabic first name is invalid");

            if (!string.IsNullOrWhiteSpace(command.LastNameAr) && !command.LastNameAr.IsValidName(true))
                errors.Add(culture == "ar" ? "اسم العائلة باللغة العربية غير صحيح" : "Arabic last name is invalid");

            // Validate email
            if (!command.Email.IsValidEmail())
                errors.Add(culture == "ar" ? "البريد الإلكتروني غير صحيح" : "Email is invalid");

            // Validate student number
            if (!command.StudentNumber.IsValidStudentNumber())
                errors.Add(culture == "ar" ? "رقم الطالب يجب أن يكون 8 أرقام" : "Student number must be 8 digits");

            // Validate date of birth
            if (!command.DateOfBirth.IsValidDateOfBirth())
                errors.Add(culture == "ar" ? "تاريخ الميلاد لا يمكن أن يكون في المستقبل" : "Date of birth cannot be in the future");

            if (!command.DateOfBirth.IsValidAge())
                errors.Add(culture == "ar" ? "عمر الطالب يجب أن يكون بين 16 و 100 سنة" : "Student age must be between 16 and 100 years");

            return (errors.Count == 0, errors);
        }

        /// <summary>
        /// Validates UpdateStudentCommand
        /// </summary>
        public static (bool IsValid, List<string> Errors) ValidateCommand(this UpdateStudentCommand command, string culture = "en")
        {
            var errors = new List<string>();

            // Validate English names
            if (!command.FirstName.IsValidName(false))
                errors.Add(culture == "ar" ? "الاسم الأول باللغة الإنجليزية غير صحيح" : "First name is invalid");

            if (!command.LastName.IsValidName(false))
                errors.Add(culture == "ar" ? "اسم العائلة باللغة الإنجليزية غير صحيح" : "Last name is invalid");

            // Validate Arabic names if provided
            if (!string.IsNullOrWhiteSpace(command.FirstNameAr) && !command.FirstNameAr.IsValidName(true))
                errors.Add(culture == "ar" ? "الاسم الأول باللغة العربية غير صحيح" : "Arabic first name is invalid");

            if (!string.IsNullOrWhiteSpace(command.LastNameAr) && !command.LastNameAr.IsValidName(true))
                errors.Add(culture == "ar" ? "اسم العائلة باللغة العربية غير صحيح" : "Arabic last name is invalid");

            // Validate email
            if (!command.Email.IsValidEmail())
                errors.Add(culture == "ar" ? "البريد الإلكتروني غير صحيح" : "Email is invalid");

            // Validate date of birth
            if (!command.DateOfBirth.IsValidDateOfBirth())
                errors.Add(culture == "ar" ? "تاريخ الميلاد لا يمكن أن يكون في المستقبل" : "Date of birth cannot be in the future");

            if (!command.DateOfBirth.IsValidAge())
                errors.Add(culture == "ar" ? "عمر الطالب يجب أن يكون بين 16 و 100 سنة" : "Student age must be between 16 and 100 years");

            return (errors.Count == 0, errors);
        }

        #endregion
    }
}
