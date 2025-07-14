using System;
using System.Threading;
using System.Threading.Tasks;
using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using MediatR;

namespace Academix.Application.Features.Dashboard.Commands.AddSkills
{
    public class AddSkillsCommandHandler : IRequestHandler<AddSkillsCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizationService;

        public AddSkillsCommandHandler(
            IUnitOfWork unitOfWork,
            ILocalizationService localizationService)
        {
            _unitOfWork = unitOfWork;
            _localizationService = localizationService;
        }

        public async Task<Result> Handle(AddSkillsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if skill already exists (case-insensitive)
                var exists = await _unitOfWork.Skills
                    .AnyAsync(s => s.NameAr.ToLower() == request.NameAr.ToLower());

                if (exists)
                {
                    var message = _localizationService.GetLocalizedString("SkillAlreadyExists");
                    return Result.Failure(message);
                }

                // Create and save new skill
                var skill = new Skill
                {
                    NameAr = request.NameAr,
                    NameEn = string.Empty
                };

                await _unitOfWork.Skills.AddAsync(skill);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var successMessage = _localizationService.GetLocalizedString("SkillAddedSuccessfully");
                return Result.Success(successMessage);
            }
            catch (Exception ex)
            {
                var errorMessage = _localizationService.GetLocalizedString("SkillAddFailed");
                return Result.Failure($"{errorMessage}: {ex.InnerException?.Message ?? ex.Message}");

            }
        }
    }
}
