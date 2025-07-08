using Academix.Application.Common.Extensions;
using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Enums;
using MediatR;

namespace Academix.Application.Features.Common.Queries.GetGenders;

public class GetGendersQueryHandler : IRequestHandler<GetGendersQuery, Result<List<GenderDto>>>
{
    private readonly ILocalizationService _localizationService;

    public GetGendersQueryHandler(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<Result<List<GenderDto>>> Handle(GetGendersQuery request, CancellationToken cancellationToken)
    {
        var genders = Enum.GetValues<Gender>().Select(gender => new GenderDto
        {
            Value = (int)gender,
            Name = gender.GetLocalizedName(_localizationService),
            NameEn = gender switch
            {
                Gender.Male => _localizationService.GetLocalizedString("Enum_Gender_Male"),
                Gender.Female => _localizationService.GetLocalizedString("Enum_Gender_Female"),
                _ => gender.ToString()
            },
            NameAr = gender switch
            {
                Gender.Male => _localizationService.GetLocalizedString("Enum_Gender_Male"),
                Gender.Female => _localizationService.GetLocalizedString("Enum_Gender_Female"),
                _ => gender.ToString()
            }
        }).ToList();

        return await Task.FromResult(Result<List<GenderDto>>.Success(genders, _localizationService.GetLocalizedString("OperationCompleted")));
    }
} 