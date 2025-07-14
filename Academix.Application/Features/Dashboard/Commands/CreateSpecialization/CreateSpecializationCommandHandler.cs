using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using MediatR;

namespace Academix.Application.Features.Dashboard.Commands.CreateSpecialization;

public class CreateSpecializationCommandHandler : IRequestHandler<CreateSpecializationCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocalizationService _localizationService;

    public CreateSpecializationCommandHandler(
        IUnitOfWork unitOfWork,
        ILocalizationService localizationService)
    {
        _unitOfWork = unitOfWork;
        _localizationService = localizationService;
    }

    public async Task<Result> Handle(CreateSpecializationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if specialization already exists (case-insensitive)
            var existsAr = await _unitOfWork.Specializations
                .AnyAsync(s => s.NameAr.ToLower() == request.NameAr.ToLower());

            var existsEn = await _unitOfWork.Specializations
                .AnyAsync(s => s.NameEn.ToLower() == request.NameEn.ToLower());

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

            // Create and save new specialization
            var specialization = new Specialization
            {
                NameAr = request.NameAr.Trim(),
                NameEn = request.NameEn.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Specializations.AddAsync(specialization);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var successMessage = _localizationService.GetLocalizedString("SpecializationCreatedSuccessfully");
            return Result.Success(successMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = _localizationService.GetLocalizedString("SpecializationCreateFailed");
            return Result.Failure($"{errorMessage}: {ex.InnerException?.Message ?? ex.Message}");
        }
    }
} 