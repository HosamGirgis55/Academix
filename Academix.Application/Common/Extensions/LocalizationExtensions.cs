using Academix.Application.Common.Interfaces;
using Academix.Domain.Entities;
using Academix.Domain.Enums;

namespace Academix.Application.Common.Extensions;

public static class LocalizationExtensions
{
     
     public static string GetLocalizedName(this Country? country, ILocalizationService localizationService)
    {
        if (country == null) return string.Empty;
        
        var currentCulture = localizationService.GetCurrentCulture();
        return currentCulture.StartsWith("ar", StringComparison.OrdinalIgnoreCase) 
            ? country.NameAr ?? country.NameEn ?? string.Empty
            : country.NameEn ?? country.NameAr ?? string.Empty;
    }

    
    public static string GetLocalizedText(this ILocalizationService localizationService, string? arabicText, string? englishText)
    {
        var currentCulture = localizationService.GetCurrentCulture();
        return currentCulture.StartsWith("ar", StringComparison.OrdinalIgnoreCase) 
            ? arabicText ?? englishText ?? string.Empty
            : englishText ?? arabicText ?? string.Empty;
    }

    
    public static string GetLocalizedName(this Gender gender, ILocalizationService localizationService)
    {
        var currentCulture = localizationService.GetCurrentCulture();
        
        return gender switch
        {
            Gender.Male => currentCulture.StartsWith("ar", StringComparison.OrdinalIgnoreCase) 
                ? localizationService.GetLocalizedString("Male_Ar") 
                : localizationService.GetLocalizedString("Male_En"),
            Gender.Female => currentCulture.StartsWith("ar", StringComparison.OrdinalIgnoreCase) 
                ? localizationService.GetLocalizedString("Female_Ar") 
                : localizationService.GetLocalizedString("Female_En"),
            _ => gender.ToString()
        };
    }
} 