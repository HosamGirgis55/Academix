using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Interfaces;
using MediatR;

namespace Academix.Application.Features.Dashboard.Commands.UpdateSkill;

public class UpdateSkillCommandHandler : IRequestHandler<UpdateSkillCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocalizationService _localizationService;

    public UpdateSkillCommandHandler(
        IUnitOfWork unitOfWork,
        ILocalizationService localizationService)
    {
        _unitOfWork = unitOfWork;
        _localizationService = localizationService;
    }

    public async Task<Result> Handle(UpdateSkillCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing skill
            var skill = await _unitOfWork.Skills.GetByIdAsync(request.Id);
            if (skill == null)
            {
                var notFoundMessage = _localizationService.GetLocalizedString("SkillNotFound");
                return Result.Failure(notFoundMessage);
            }

            // Check if new names already exist in other records
            var existsAr = await _unitOfWork.Skills
                .AnyAsync(s => s.Id != request.Id && s.NameAr.ToLower() == request.NameAr.ToLower());

            var existsEn = await _unitOfWork.Skills
                .AnyAsync(s => s.Id != request.Id && s.NameEn.ToLower() == request.NameEn.ToLower());

            if (existsAr)
            {
                var message = _localizationService.GetLocalizedString("SkillArAlreadyExists");
                return Result.Failure(message);
            }

            if (existsEn)
            {
                var message = _localizationService.GetLocalizedString("SkillEnAlreadyExists");
                return Result.Failure(message);
            }

            // Update skill properties
            skill.NameAr = request.NameAr.Trim();
            skill.NameEn = request.NameEn.Trim();
            skill.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Skills.Update(skill);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var successMessage = _localizationService.GetLocalizedString("SkillUpdatedSuccessfully");
            return Result.Success(successMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = _localizationService.GetLocalizedString("SkillUpdateFailed");
            return Result.Failure($"{errorMessage}: {ex.InnerException?.Message ?? ex.Message}");
        }
    }
} 