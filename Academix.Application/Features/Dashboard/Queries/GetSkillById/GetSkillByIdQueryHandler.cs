using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using Academix.Domain.Interfaces;
using MediatR;

namespace Academix.Application.Features.Dashboard.Queries.GetSkillById;

public class GetSkillByIdQueryHandler : IRequestHandler<GetSkillByIdQuery, Result<SkillDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocalizationService _localizationService;

    public GetSkillByIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILocalizationService localizationService)
    {
        _unitOfWork = unitOfWork;
        _localizationService = localizationService;
    }

    public async Task<Result<SkillDto>> Handle(GetSkillByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var skill = await _unitOfWork.Skills.GetByIdAsync(request.Id);
            if (skill == null)
            {
                var notFoundMessage = _localizationService.GetLocalizedString("SkillNotFound");
                return Result<SkillDto>.Failure(notFoundMessage);
            }

            // Get student and teacher counts for this skill
            var studentSkillsQuery = await _unitOfWork.Repository<Domain.Entities.StudentSkill>().GetAllAsync();
            var teacherSkillsQuery = await _unitOfWork.Repository<Domain.Entities.TeacherSkill>().GetAllAsync();
            
            var studentCount = studentSkillsQuery.Count(ss => ss.SkillId == request.Id);
            var teacherCount = teacherSkillsQuery.Count(ts => ts.SkillId == request.Id);

            var skillDto = new SkillDto
            {
                Id = skill.Id,
                NameAr = skill.NameAr,
                NameEn = skill.NameEn,
                StudentCount = studentCount,
                TeacherCount = teacherCount,
                CreatedAt = skill.CreatedAt,
                UpdatedAt = skill.UpdatedAt
            };

            var successMessage = _localizationService.GetLocalizedString("SkillRetrievedSuccessfully");
            return Result<SkillDto>.Success(skillDto, successMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = _localizationService.GetLocalizedString("SkillRetrievalFailed");
            return Result<SkillDto>.Failure($"{errorMessage}: {ex.InnerException?.Message ?? ex.Message}");
        }
    }
} 