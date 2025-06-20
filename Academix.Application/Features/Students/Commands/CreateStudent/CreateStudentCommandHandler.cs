using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using AutoMapper;
using Academix.Application.Common.Mappings;

namespace Academix.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentCommandHandler : ICommandHandler<CreateStudentCommand, Result<Guid>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateStudentCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            // Check if email already exists using extension method
            var existingStudents = await _studentRepository.GetAllAsync();
            if (!existingStudents.AsQueryable().IsEmailUnique(request.Email))
            {
                return Result<Guid>.Failure("A student with this email already exists.");
            }

            // Check if student number already exists using extension method
            if (!existingStudents.AsQueryable().IsStudentNumberUnique(request.StudentNumber))
            {
                return Result<Guid>.Failure("A student with this student number already exists.");
            }

            // Create student entity using extension method
            var student = request.ToStudentEntity(_mapper);

            // Add to repository
            await _studentRepository.AddAsync(student);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(student.Id);
        }
    }
} 