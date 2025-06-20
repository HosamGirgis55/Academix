using AutoMapper;
using Academix.Domain.Entities;
using Academix.Application.Features.Students.Queries.GetStudentById;
using Academix.Application.Features.Students.Queries.GetStudentsList;
using Academix.Application.Features.Students.Commands.CreateStudent;
using Academix.Application.Features.Students.Commands.UpdateStudent;

namespace Academix.Application.Common.Mappings
{
    public static class MappingExtensions
    {
        #region Student Entity Extensions

        /// <summary>
        /// Maps Student entity to StudentDto with culture-specific localization
        /// </summary>
        public static StudentDto ToStudentDto(this Student student, IMapper mapper, string culture = "en")
        {
            var dto = mapper.Map<StudentDto>(student);
            dto.FirstName = student.GetFirstName(culture);
            dto.LastName = student.GetLastName(culture);
            dto.FullName = student.GetFullName(culture);
            return dto;
        }

        /// <summary>
        /// Maps Student entity to StudentListDto with culture-specific localization
        /// </summary>
        public static StudentListDto ToStudentListDto(this Student student, IMapper mapper, string culture = "en")
        {
            var dto = mapper.Map<StudentListDto>(student);
            dto.FirstName = student.GetFirstName(culture);
            dto.LastName = student.GetLastName(culture);
            dto.FullName = student.GetFullName(culture);
            dto.AgeGroup = student.DateOfBirth.GetAgeGroup(culture);
            return dto;
        }

        /// <summary>
        /// Updates existing Student entity from UpdateStudentCommand
        /// </summary>
        public static Student UpdateFrom(this Student student, UpdateStudentCommand command, IMapper mapper)
        {
            mapper.Map(command, student);
            student.UpdatedAt = DateTime.UtcNow;
            return student;
        }

        /// <summary>
        /// Creates new Student entity from CreateStudentCommand
        /// </summary>
        public static Student ToStudentEntity(this CreateStudentCommand command, IMapper mapper)
        {
            var student = mapper.Map<Student>(command);
            student.Id = Guid.NewGuid();
            student.CreatedAt = DateTime.UtcNow;
            return student;
        }

        #endregion

        #region Collection Extensions

        /// <summary>
        /// Maps collection of Students to StudentDto list with culture support
        /// </summary>
        public static List<StudentDto> ToStudentDtoList(this IEnumerable<Student> students, IMapper mapper, string culture = "en")
        {
            return students.Select(s => s.ToStudentDto(mapper, culture)).ToList();
        }

        /// <summary>
        /// Maps collection of Students to StudentListDto list with culture support
        /// </summary>
        public static List<StudentListDto> ToStudentListDtoList(this IEnumerable<Student> students, IMapper mapper, string culture = "en")
        {
            return students.Select(s => s.ToStudentListDto(mapper, culture)).ToList();
        }

        /// <summary>
        /// Projects IQueryable of Students to StudentDto with culture support using AutoMapper ProjectTo
        /// </summary>
        public static IQueryable<StudentDto> ProjectToStudentDto(this IQueryable<Student> students, IMapper mapper, string culture = "en")
        {
            return students.ProjectToWithCulture<StudentDto>(mapper, culture);
        }

        /// <summary>
        /// Projects IQueryable of Students to StudentListDto with culture support using AutoMapper ProjectTo
        /// </summary>
        public static IQueryable<StudentListDto> ProjectToStudentListDto(this IQueryable<Student> students, IMapper mapper, string culture = "en")
        {
            return students.ProjectToWithCulture<StudentListDto>(mapper, culture);
        }

        /// <summary>
        /// Gets paginated result of Students mapped to StudentDto
        /// </summary>
        public static async Task<PaginatedResult<StudentDto>> ToPaginatedStudentDtoAsync(
            this IQueryable<Student> students, 
            IMapper mapper, 
            int pageNumber, 
            int pageSize, 
            string culture = "en",
            CancellationToken cancellationToken = default)
        {
            var projectedQuery = students.ProjectToStudentDto(mapper, culture);
            return await projectedQuery.ToPaginatedListAsync(pageNumber, pageSize, culture, cancellationToken);
        }

        /// <summary>
        /// Gets paginated result of Students mapped to StudentListDto
        /// </summary>
        public static async Task<PaginatedResult<StudentListDto>> ToPaginatedStudentListDtoAsync(
            this IQueryable<Student> students, 
            IMapper mapper, 
            int pageNumber, 
            int pageSize, 
            string culture = "en",
            CancellationToken cancellationToken = default)
        {
            var projectedQuery = students.ProjectToStudentListDto(mapper, culture);
            return await projectedQuery.ToPaginatedListAsync(pageNumber, pageSize, culture, cancellationToken);
        }

        #endregion

        #region Search and Filter Extensions

        /// <summary>
        /// Applies search filter to Student query with culture-aware name searching
        /// </summary>
        public static IQueryable<Student> SearchByTerm(this IQueryable<Student> students, string searchTerm, string culture = "en")
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return students;

            return students.SearchInCultureFields(searchTerm, culture, "FirstName", "LastName", "Email", "StudentNumber");
        }

        /// <summary>
        /// Applies ordering to Student query with culture-aware name ordering
        /// </summary>
        public static IQueryable<Student> OrderByField(this IQueryable<Student> students, string orderBy, string culture = "en")
        {
            if (string.IsNullOrWhiteSpace(orderBy))
                return students.OrderByCultureField("LastName", culture);

            return orderBy.ToLower() switch
            {
                "firstname" => students.OrderByCultureField("FirstName", culture),
                "lastname" => students.OrderByCultureField("LastName", culture),
                "email" => students.OrderBy(s => s.Email),
                "studentnumber" => students.OrderBy(s => s.StudentNumber),
                "dateofbirth" => students.OrderBy(s => s.DateOfBirth),
                "createdat" => students.OrderBy(s => s.CreatedAt),
                _ => students.OrderByCultureField("LastName", culture)
            };
        }

        /// <summary>
        /// Comprehensive student query with search, ordering, and pagination
        /// </summary>
        public static async Task<PaginatedResult<StudentListDto>> GetStudentsWithFiltersAsync(
            this IQueryable<Student> students,
            IMapper mapper,
            string? searchTerm = null,
            string? orderBy = null,
            int pageNumber = 1,
            int pageSize = 10,
            string culture = "en",
            CancellationToken cancellationToken = default)
        {
            var query = students
                .SearchByTerm(searchTerm ?? string.Empty, culture)
                .OrderByField(orderBy ?? string.Empty, culture);

            return await query.ToPaginatedStudentListDtoAsync(mapper, pageNumber, pageSize, culture, cancellationToken);
        }

        #endregion

        #region Validation Extensions

        /// <summary>
        /// Checks if email is unique among students (excluding current student if updating)
        /// </summary>
        public static bool IsEmailUnique(this IQueryable<Student> students, string email, Guid? excludeId = null)
        {
            var query = students.Where(s => s.Email.ToLower() == email.ToLower());
            
            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);
                
            return !query.Any();
        }

        /// <summary>
        /// Checks if student number is unique among students (excluding current student if updating)
        /// </summary>
        public static bool IsStudentNumberUnique(this IQueryable<Student> students, string studentNumber, Guid? excludeId = null)
        {
            var query = students.Where(s => s.StudentNumber.ToLower() == studentNumber.ToLower());
            
            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);
                
            return !query.Any();
        }

        #endregion

        #region Age Calculation Extensions

        /// <summary>
        /// Calculates age from date of birth
        /// </summary>
        public static int CalculateAge(this DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age))
                age--;
            return age;
        }

        /// <summary>
        /// Gets age group description based on age
        /// </summary>
        public static string GetAgeGroup(this DateTime dateOfBirth, string culture = "en")
        {
            var age = dateOfBirth.CalculateAge();
            
            return age switch
            {
                < 18 => culture == "ar" ? "????" : "Minor",
                >= 18 and < 25 => culture == "ar" ? "???" : "Young Adult",
                >= 25 and < 35 => culture == "ar" ? "????" : "Adult",
                >= 35 and < 50 => culture == "ar" ? "????? ?????" : "Middle-aged",
                >= 50 => culture == "ar" ? "???? ????" : "Senior"
            };
        }

        #endregion

        #region Bulk Operations Extensions

        /// <summary>
        /// Bulk map students to DTOs using parallel processing with culture support
        /// </summary>
        public static List<StudentDto> ToStudentDtoListParallel(this IEnumerable<Student> students, IMapper mapper, string culture = "en")
        {
            return students.MapToParallel<Student, StudentDto>(mapper, culture).ToList();
        }

        /// <summary>
        /// Async bulk mapping with culture support
        /// </summary>
        public static async Task<List<StudentDto>> ToStudentDtoListAsync(
            this IEnumerable<Student> students, 
            IMapper mapper, 
            string culture = "en",
            CancellationToken cancellationToken = default)
        {
            return await students.MapToListAsync<Student, StudentDto>(mapper, culture, cancellationToken);
        }

        #endregion
    }
}
