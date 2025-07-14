using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academix.Application.Features.Dashboard.Commands.DeleteSkill;

public class DeleteSkillCommandHandler : IRequestHandler<DeleteSkillCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocalizationService _localizationService;

    public DeleteSkillCommandHandler(
        IUnitOfWork unitOfWork,
        ILocalizationService localizationService)
    {
        _unitOfWork = unitOfWork;
        _localizationService = localizationService;
    }

    public async Task<Result> Handle(DeleteSkillCommand request, CancellationToken cancellationToken)
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

            // Check if skill is used by students or teachers
            var studentSkillsQuery = await _unitOfWork.Repository<Domain.Entities.StudentSkill>().GetAllAsync();
            var teacherSkillsQuery = await _unitOfWork.Repository<Domain.Entities.TeacherSkill>().GetAllAsync();
            
            var hasStudents = studentSkillsQuery.Any(ss => ss.SkillId == request.Id);
            var hasTeachers = teacherSkillsQuery.Any(ts => ts.SkillId == request.Id);

            if (hasStudents || hasTeachers)
            {
                var message = _localizationService.GetLocalizedString("SkillHasUsersCannotDelete");
                return Result.Failure(message);
            }

            // Delete skill
            _unitOfWork.Skills.Delete(skill);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var successMessage = _localizationService.GetLocalizedString("SkillDeletedSuccessfully");
            return Result.Success(successMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = _localizationService.GetLocalizedString("SkillDeleteFailed");
            return Result.Failure($"{errorMessage}: {ex.InnerException?.Message ?? ex.Message}");
        }
    }
} 