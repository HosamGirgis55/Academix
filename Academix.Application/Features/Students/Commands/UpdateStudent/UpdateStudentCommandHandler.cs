using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Interfaces;
using AutoMapper;
using Academix.Application.Common.Mappings;

namespace Academix.Application.Features.Students.Commands.UpdateStudent
{
    public class UpdateStudentCommandHandler : ICommandHandler<UpdateStudentCommand, Result<bool>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateStudentCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<bool>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.Id);
            
            if (student == null)
            {
                return Result<bool>.Failure($"Student with ID {request.Id} not found.");
            }

            // Check if email is being changed and already exists using extension method
            if (student.Email != request.Email)
            {
                var existingStudents = await _studentRepository.GetAllAsync();
                if (!existingStudents.AsQueryable().IsEmailUnique(request.Email, request.Id))
                {
                    return Result<bool>.Failure("A student with this email already exists.");
                }
            }

            // Update student using extension method
            student.UpdateFrom(request, _mapper);

            _studentRepository.Update(student);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
} 