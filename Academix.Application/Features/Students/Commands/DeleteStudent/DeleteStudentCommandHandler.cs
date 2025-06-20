using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Interfaces;

namespace Academix.Application.Features.Students.Commands.DeleteStudent
{
    public class DeleteStudentCommandHandler : ICommandHandler<DeleteStudentCommand, Result>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteStudentCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork)
        {
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.Id);
            
            if (student == null)
            {
                return Result.Failure($"Student with ID {request.Id} not found.");
            }

            _studentRepository.Delete(student);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
} 