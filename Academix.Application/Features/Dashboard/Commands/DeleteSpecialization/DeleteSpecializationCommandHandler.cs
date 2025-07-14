using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academix.Application.Features.Dashboard.Commands.DeleteSpecialization;

public class DeleteSpecializationCommandHandler : IRequestHandler<DeleteSpecializationCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocalizationService _localizationService;

    public DeleteSpecializationCommandHandler(
        IUnitOfWork unitOfWork,
        ILocalizationService localizationService)
    {
        _unitOfWork = unitOfWork;
        _localizationService = localizationService;
    }

    public async Task<Result> Handle(DeleteSpecializationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing specialization
            var specialization = await _unitOfWork.Specializations.GetByIdAsync(request.Id);
            if (specialization == null)
            {
                var notFoundMessage = _localizationService.GetLocalizedString("SpecializationNotFound");
                return Result.Failure(notFoundMessage);
            }

            // Check if specialization is used by students
            var studentsQuery = await _unitOfWork.Students.GetAllAsync();
            var hasStudents = studentsQuery.Any(s => s.SpecialistId == request.Id);

            if (hasStudents)
            {
                var message = _localizationService.GetLocalizedString("SpecializationHasStudentsCannotDelete");
                return Result.Failure(message);
            }

            // Delete specialization
            _unitOfWork.Specializations.Delete(specialization);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var successMessage = _localizationService.GetLocalizedString("SpecializationDeletedSuccessfully");
            return Result.Success(successMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = _localizationService.GetLocalizedString("SpecializationDeleteFailed");
            return Result.Failure($"{errorMessage}: {ex.InnerException?.Message ?? ex.Message}");
        }
    }
} 