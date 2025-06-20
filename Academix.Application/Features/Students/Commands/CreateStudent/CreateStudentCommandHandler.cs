using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;

namespace Academix.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentCommandHandler : ICommandHandler<CreateStudentCommand, Result<Guid>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateStudentCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork)
        {
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            // Check if student with same email exists
            var existingStudent = await _studentRepository.GetAllAsync();
            if (existingStudent != null)
            {
                return Result<Guid>.Failure("A student with this email already exists.");
            }

            // Create the student entity
            var student = new Student
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                FirstNameAr = request.FirstNameAr,
                LastName = request.LastName,
                LastNameAr = request.LastNameAr,
                Email = request.Email,
                DateOfBirth = request.DateOfBirth,
                StudentNumber = request.StudentNumber,
                CreatedAt = DateTime.UtcNow
            };

            // Add to repository
            await _studentRepository.AddAsync(student);

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(student.Id);
        }
    }
} 