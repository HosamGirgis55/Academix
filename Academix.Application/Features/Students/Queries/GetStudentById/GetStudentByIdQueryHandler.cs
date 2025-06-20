using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Academix.Application.Features.Students.Queries.GetStudentById
{
    public class GetStudentByIdQueryHandler : IQueryHandler<GetStudentByIdQuery, Result<StudentDto>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetStudentByIdQueryHandler(IStudentRepository studentRepository, IHttpContextAccessor httpContextAccessor)
        {
            _studentRepository = studentRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<StudentDto>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId);
            
            if (student == null)
            {
                return Result<StudentDto>.Failure($"Student with ID {request.StudentId} not found.");
            }

            // Get culture from HttpContext
            var culture = _httpContextAccessor.HttpContext?.Items["Culture"]?.ToString() ?? "en";

            var studentDto = new StudentDto
            {
                Id = student.Id,
                FirstName = student.GetFirstName(culture),
                LastName = student.GetLastName(culture),
                FullName = student.GetFullName(culture),
                Email = student.Email,
                DateOfBirth = student.DateOfBirth,
                StudentNumber = student.StudentNumber,
                CreatedAt = student.CreatedAt,
                UpdatedAt = student.UpdatedAt
            };
            
            return Result<StudentDto>.Success(studentDto);
        }
    }
} 