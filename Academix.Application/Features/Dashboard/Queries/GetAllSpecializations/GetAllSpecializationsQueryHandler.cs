using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using Academix.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academix.Application.Features.Dashboard.Queries.GetAllSpecializations;

public class GetAllSpecializationsQueryHandler : IRequestHandler<GetAllSpecializationsQuery, Result<List<SpecializationDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocalizationService _localizationService;

    public GetAllSpecializationsQueryHandler(
        IUnitOfWork unitOfWork,
        ILocalizationService localizationService)
    {
        _unitOfWork = unitOfWork;
        _localizationService = localizationService;
    }

    public async Task<Result<List<SpecializationDto>>> Handle(GetAllSpecializationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var specializationsQuery = await _unitOfWork.Specializations.GetAllAsync();
            var specializations = specializationsQuery.ToList();

            if (!specializations.Any())
            {
                var emptyMessage = _localizationService.GetLocalizedString("NoSpecializationsFound");
                return Result<List<SpecializationDto>>.Success(new List<SpecializationDto>(), emptyMessage);
            }

            // Get student counts for each specialization
            var studentsQuery = await _unitOfWork.Students.GetAllAsync();
            var students = studentsQuery.ToList();

            var specializationDtos = specializations.Select(s => new SpecializationDto
            {
                Id = s.Id,
                NameAr = s.NameAr,
                NameEn = s.NameEn,
                StudentCount = students.Count(st => st.SpecialistId == s.Id),
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).OrderBy(s => s.NameEn).ToList();

            var successMessage = _localizationService.GetLocalizedString("SpecializationsRetrievedSuccessfully");
            return Result<List<SpecializationDto>>.Success(specializationDtos, successMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = _localizationService.GetLocalizedString("SpecializationsRetrieveFailed");
            return Result<List<SpecializationDto>>.Failure($"{errorMessage}: {ex.InnerException?.Message ?? ex.Message}");
        }
    }
} 