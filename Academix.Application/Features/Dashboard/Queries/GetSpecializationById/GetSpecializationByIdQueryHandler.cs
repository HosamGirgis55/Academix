using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using Academix.Domain.Interfaces;
using MediatR;

namespace Academix.Application.Features.Dashboard.Queries.GetSpecializationById;

public class GetSpecializationByIdQueryHandler : IRequestHandler<GetSpecializationByIdQuery, Result<SpecializationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocalizationService _localizationService;

    public GetSpecializationByIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILocalizationService localizationService)
    {
        _unitOfWork = unitOfWork;
        _localizationService = localizationService;
    }

    public async Task<Result<SpecializationDto>> Handle(GetSpecializationByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var specialization = await _unitOfWork.Specializations.GetByIdAsync(request.Id);
            if (specialization == null)
            {
                var notFoundMessage = _localizationService.GetLocalizedString("SpecializationNotFound");
                return Result<SpecializationDto>.Failure(notFoundMessage);
            }

            // Get student count for this specialization
            var studentsQuery = await _unitOfWork.Students.GetAllAsync();
            var studentCount = studentsQuery.Count(s => s.SpecialistId == request.Id);

            var specializationDto = new SpecializationDto
            {
                Id = specialization.Id,
                NameAr = specialization.NameAr,
                NameEn = specialization.NameEn,
                StudentCount = studentCount,
                CreatedAt = specialization.CreatedAt,
                UpdatedAt = specialization.UpdatedAt
            };

            var successMessage = _localizationService.GetLocalizedString("SpecializationRetrievedSuccessfully");
            return Result<SpecializationDto>.Success(specializationDto, successMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = _localizationService.GetLocalizedString("SpecializationRetrieveFailed");
            return Result<SpecializationDto>.Failure($"{errorMessage}: {ex.InnerException?.Message ?? ex.Message}");
        }
    }
} 