using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using Academix.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academix.Application.Features.Dashboard.Queries.GetAllSkills;

public class GetAllSkillsQueryHandler : IRequestHandler<GetAllSkillsQuery, Result<List<SkillDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocalizationService _localizationService;

    public GetAllSkillsQueryHandler(
        IUnitOfWork unitOfWork,
        ILocalizationService localizationService)
    {
        _unitOfWork = unitOfWork;
        _localizationService = localizationService;
    }

    public async Task<Result<List<SkillDto>>> Handle(GetAllSkillsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var skillsQuery = await _unitOfWork.Skills.GetAllAsync();
            var skills = skillsQuery.ToList();

            if (!skills.Any())
            {
                var emptyMessage = _localizationService.GetLocalizedString("NoSkillsFound");
                return Result<List<SkillDto>>.Success(new List<SkillDto>(), emptyMessage);
            }

            // Get student and teacher skill counts
            var studentSkillsQuery = await _unitOfWork.Repository<Domain.Entities.StudentSkill>().GetAllAsync();
            var teacherSkillsQuery = await _unitOfWork.Repository<Domain.Entities.TeacherSkill>().GetAllAsync();
            
            var studentSkills = studentSkillsQuery.ToList();
            var teacherSkills = teacherSkillsQuery.ToList();

            var skillDtos = skills.Select(s => new SkillDto
            {
                Id = s.Id,
                NameAr = s.NameAr,
                NameEn = s.NameEn,
                StudentCount = studentSkills.Count(ss => ss.SkillId == s.Id),
                TeacherCount = teacherSkills.Count(ts => ts.SkillId == s.Id),
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).OrderBy(s => s.NameEn).ToList();

            var successMessage = _localizationService.GetLocalizedString("SkillsRetrievedSuccessfully");
            return Result<List<SkillDto>>.Success(skillDtos, successMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = _localizationService.GetLocalizedString("SkillsRetrievalFailed");
            return Result<List<SkillDto>>.Failure($"{errorMessage}: {ex.InnerException?.Message ?? ex.Message}");
        }
    }
} 