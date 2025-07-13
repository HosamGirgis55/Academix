using Academix.Application.Common.Extensions;
using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using Academix.Domain.Entities;
using Academix.Domain.Enums;
using Academix.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Academix.Application.Features.Common.Queries.GetLookup;

public class GetLookupQueryHandler : IRequestHandler<GetLookupQuery, Result<List<LookupItemDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocalizationService _localizationService;

    public GetLookupQueryHandler(
        IUnitOfWork unitOfWork,
        ILocalizationService localizationService)
    {
        _unitOfWork = unitOfWork;
        _localizationService = localizationService;
    }

    public async Task<Result<List<LookupItemDto>>> Handle(GetLookupQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var items = request.Type.ToLower() switch
            {
                "gender" => GetGenderLookup(),
                "country" => await GetCountryLookup(cancellationToken),
                "field" => await GetFieldLookup(cancellationToken),
                "level" => await GetLevelLookup(cancellationToken),
                "position" => await GetPositionLookup(cancellationToken),
                "specialization" => await GetSpecializationLookup(cancellationToken),
                "teachingarea" => await GetTeachingAreaLookup(cancellationToken),
                "teachinglanguage" => await GetTeachingLanguageLookup(cancellationToken),
                "communicationmethod" => await GetCommunicationMethodLookup(cancellationToken),
                "agegroup" => await GetAgeGroupLookup(cancellationToken),
                "graduationstatus" => await GetGraduationStatusLookup(cancellationToken),
                "learninginterest" => await GetLearningInterestLookup(cancellationToken),
                "experiences" => await GetExperienceLookup(cancellationToken),
                "skilles" => await GetSkilleLookup(cancellationToken),
                _ => new List<LookupItemDto>()
            };

            return Result<List<LookupItemDto>>.Success(items, _localizationService.GetLocalizedString("OperationCompleted"));
        }
        catch (Exception ex)
        {
            return Result<List<LookupItemDto>>.Failure($"Error getting lookup data: {ex.Message}");
        }
    }

    private List<LookupItemDto> GetGenderLookup()
    {
        return Enum.GetValues<Gender>()
            .Select(value => new LookupItemDto
            {
                Id = (int)value,
                Name = value.ToString()
            })
            .ToList();
    }

    private async Task<List<LookupItemDto>> GetCountryLookup(CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.Countries.GetAllAsync();
        var countries = await query.AsNoTracking().ToListAsync(cancellationToken);
        
        return countries.Select(country => new LookupItemDto
        {
            Id = country.Id.ToString(),
            Name = GetLocalizedName(country.NameEn, country.NameAr)
        }).ToList();
    }

    private async Task<List<LookupItemDto>> GetFieldLookup(CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.Repository<Field>().GetAllAsync();
        var fields = await query.AsNoTracking().ToListAsync(cancellationToken);
        
        return fields.Select(field => new LookupItemDto
        {
            Id = field.Id.ToString(),
            Name = GetLocalizedName(field.NameEn, field.NameAr)
        }).ToList();
    }

    private async Task<List<LookupItemDto>> GetLevelLookup(CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.Levels.GetAllAsync();
        var levels = await query.AsNoTracking().ToListAsync(cancellationToken);
        
        return levels.Select(level => new LookupItemDto
        {
            Id = level.Id.ToString(),
            Name = GetLocalizedName(level.NameEn, level.NameAr)
        }).ToList();
    }



    private async Task<List<LookupItemDto>> GetPositionLookup(CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.Repository<Position>().GetAllAsync();
        var positions = await query.AsNoTracking().ToListAsync(cancellationToken);
        
        return positions.Select(position => new LookupItemDto
        {
            Id = position.Id.ToString(),
            Name = GetLocalizedName(position.NameEn, position.NameAr)
        }).ToList();
    }

    private async Task<List<LookupItemDto>> GetSpecializationLookup(CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.Specializations.GetAllAsync();
        var specializations = await query.AsNoTracking().ToListAsync(cancellationToken);
        
        return specializations.Select(specialization => new LookupItemDto
        {
            Id = specialization.Id.ToString(),
            Name = GetLocalizedName(specialization.NameEn, specialization.NameAr)
        }).ToList();
    }

    private async Task<List<LookupItemDto>> GetTeachingAreaLookup(CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.Repository<TeachingArea>().GetAllAsync();
        var teachingAreas = await query.AsNoTracking().ToListAsync(cancellationToken);
        
        return teachingAreas.Select(area => new LookupItemDto
        {
            Id = area.Id.ToString(),
            Name = GetLocalizedName(area.NameEn, area.NameAr)
        }).ToList();
    }

    private async Task<List<LookupItemDto>> GetTeachingLanguageLookup(CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.Repository<TeachingLanguage>().GetAllAsync();
        var teachingLanguages = await query.AsNoTracking().ToListAsync(cancellationToken);
        
        return teachingLanguages.Select(language => new LookupItemDto
        {
            Id = language.Id.ToString(),
            Name = GetLocalizedName(language.NameEn, language.NameAr)
        }).ToList();
    }

    private async Task<List<LookupItemDto>> GetCommunicationMethodLookup(CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.Repository<CommunicationMethod>().GetAllAsync();
        var methods = await query.AsNoTracking().ToListAsync(cancellationToken);
        
        return methods.Select(method => new LookupItemDto
        {
            Id = method.Id.ToString(),
            Name = GetLocalizedName(method.NameEn, method.NameAr)
        }).ToList();
    }

    private async Task<List<LookupItemDto>> GetAgeGroupLookup(CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.Repository<AgeGroup>().GetAllAsync();
        var ageGroups = await query.AsNoTracking().ToListAsync(cancellationToken);
        
        return ageGroups.Select(group => new LookupItemDto
        {
            Id = group.Id.ToString(),
            Name = GetLocalizedName(group.NameEn, group.NameAr)
        }).ToList();
    }

    private async Task<List<LookupItemDto>> GetGraduationStatusLookup(CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.Repository<GraduationStatus>().GetAllAsync();
        var ageGroups = await query.AsNoTracking().ToListAsync(cancellationToken);

        return ageGroups.Select(group => new LookupItemDto
        {
            Id = group.Id.ToString(),
            Name = GetLocalizedName(group.NameEn, group.NameAr)
        }).ToList();
    }
    private async Task<List<LookupItemDto>> GetSkilleLookup(CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.Repository<Skill>().GetAllAsync();
        var ageGroups = await query.AsNoTracking().ToListAsync(cancellationToken);

        return ageGroups.Select(group => new LookupItemDto
        {
            Id = group.Id.ToString(),
            Name = GetLocalizedName(group.NameEn, group.NameAr)
        }).ToList();
    }
    private async Task<List<LookupItemDto>> GetExperienceLookup(CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.Repository<Experience>().GetAllAsync();
        var ageGroups = await query.AsNoTracking().ToListAsync(cancellationToken);

        return ageGroups.Select(group => new LookupItemDto
        {
            Id = group.Id.ToString(),
            Name = GetLocalizedName(group.NameEn, group.NameAr)
        }).ToList();
    }

    private async Task<List<LookupItemDto>> GetLearningInterestLookup(CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.Repository<Field>().GetAllAsync();
        var learningInterests = await query.AsNoTracking().ToListAsync(cancellationToken);

        return learningInterests.Select(interest => new LookupItemDto
        {
            Id = interest.Id.ToString(),
            Name = GetLocalizedName(interest.NameEn, interest.NameAr)
        }).ToList();
    }

    private string GetLocalizedName(string nameEn, string nameAr)
    {
        var currentCulture = Thread.CurrentThread.CurrentUICulture.Name.ToLower();
        return currentCulture.StartsWith("ar") ? nameAr : nameEn;
    }
} 