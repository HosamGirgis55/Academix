using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Interfaces;

namespace Academix.Application.Features.Students.Commands.UpdateStudent
{
    public class UpdateStudentCommandHandler : ICommandHandler<UpdateStudentCommand, Result<bool>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStudentCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork)
        {
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.Id);
            
            if (student == null)
            {
                return Result<bool>.Failure($"Student with ID {request.Id} not found.");
            }

            // Check if email is being changed and already exists
            if (student.Email != request.Email)
            {
                var existingStudent = await _studentRepository.GetAllAsync();
                if (existingStudent != null && existingStudent.FirstOrDefault()?.Id != request.Id)
                {
                    return Result<bool>.Failure("A student with this email already exists.");
                }
            }

            // Update student properties
            student.FirstName = request.FirstName;
            student.FirstNameAr = request.FirstNameAr;
            student.LastName = request.LastName;
            student.LastNameAr = request.LastNameAr;
            student.Email = request.Email;
            student.DateOfBirth = request.DateOfBirth;
            student.UpdatedAt = DateTime.UtcNow;

            _studentRepository.Update(student);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
} 