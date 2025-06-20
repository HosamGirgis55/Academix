using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Academix.Application.Features.Students.Queries.GetStudentsList
{
    public class GetStudentsListQueryHandler : IQueryHandler<GetStudentsListQuery, Result<StudentsListVm>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetStudentsListQueryHandler(IStudentRepository studentRepository, IHttpContextAccessor httpContextAccessor)
        {
            _studentRepository = studentRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<StudentsListVm>> Handle(GetStudentsListQuery request, CancellationToken cancellationToken)
        {
            // Get culture from HttpContext
            var culture = _httpContextAccessor.HttpContext?.Items["Culture"]?.ToString() ?? "en";
            
            // Get all students (in a real app, this would be paginated at the database level)
            var allStudents = await _studentRepository.GetAllAsync();
            
            // Apply search filter
            var query = allStudents.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(s => 
                    s.GetFirstName(culture).ToLower().Contains(searchTerm) ||
                    s.GetLastName(culture).ToLower().Contains(searchTerm) ||
                    s.Email.ToLower().Contains(searchTerm) ||
                    s.StudentNumber.ToLower().Contains(searchTerm));
            }

            // Apply ordering
            query = request.OrderBy?.ToLower() switch
            {
                "firstname" => query.OrderBy(s => s.FirstName),
                "lastname" => query.OrderBy(s => s.LastName),
                "email" => query.OrderBy(s => s.Email),
                "studentnumber" => query.OrderBy(s => s.StudentNumber),
                _ => query.OrderBy(s => s.LastName).ThenBy(s => s.FirstName)
            };

            // Get total count
            var totalCount = query.Count();

            // Apply pagination
            var students = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Map to DTOs
            var studentDtos = students.Select(s => new StudentListDto
            {
                Id = s.Id,
                FirstName = s.GetFirstName(culture),
                LastName = s.GetLastName(culture),
                FullName = s.GetFullName(culture),
                Email = s.Email,
                StudentNumber = s.StudentNumber,
                Age = DateTime.Today.Year - s.DateOfBirth.Year - 
                      (s.DateOfBirth.Date > DateTime.Today.AddYears(-(DateTime.Today.Year - s.DateOfBirth.Year)) ? 1 : 0)
            }).ToList();

            // Create response
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
            var response = new StudentsListVm
            {
                Students = studentDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasPreviousPage = request.PageNumber > 1,
                HasNextPage = request.PageNumber < totalPages
            };

            return Result<StudentsListVm>.Success(response);
        }
    }
} 