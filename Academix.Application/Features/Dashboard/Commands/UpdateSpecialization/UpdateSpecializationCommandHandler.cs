using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Interfaces;
using MediatR;

namespace Academix.Application.Features.Dashboard.Commands.UpdateSpecialization;

public class UpdateSpecializationCommandHandler : IRequestHandler<UpdateSpecializationCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocalizationService _localizationService;

    public UpdateSpecializationCommandHandler(
        IUnitOfWork unitOfWork,
        ILocalizationService localizationService)
    {
        _unitOfWork = unitOfWork;
        _localizationService = localizationService;
    }

    public async Task<Result> Handle(UpdateSpecializationCommand request, CancellationToken cancellationToken)
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

            // Check if new names already exist in other records
            var existsAr = await _unitOfWork.Specializations
                .AnyAsync(s => s.Id != request.Id && s.NameAr.ToLower() == request.NameAr.ToLower());

            var existsEn = await _unitOfWork.Specializations
                .AnyAsync(s => s.Id != request.Id && s.NameEn.ToLower() == request.NameEn.ToLower());

            if (existsAr)
            {
                var message = _localizationService.GetLocalizedString("SpecializationArAlreadyExists");
                return Result.Failure(message);
            }

            if (existsEn)
            {
                var message = _localizationService.GetLocalizedString("SpecializationEnAlreadyExists");
                return Result.Failure(message);
            }

            // Update specialization
            specialization.NameAr = request.NameAr.Trim();
            specialization.NameEn = request.NameEn.Trim();
            specialization.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Specializations.Update(specialization);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var successMessage = _localizationService.GetLocalizedString("SpecializationUpdatedSuccessfully");
            return Result.Success(successMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = _localizationService.GetLocalizedString("SpecializationUpdateFailed");
            return Result.Failure($"{errorMessage}: {ex.InnerException?.Message ?? ex.Message}");
        }
    }
} 