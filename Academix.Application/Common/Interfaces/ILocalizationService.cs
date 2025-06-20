namespace Academix.Application.Common.Interfaces
{
    public interface ILocalizationService
    {
        string GetCurrentCulture();
        string GetLocalizedString(string key);
        string GetLocalizedString(string key, params object[] args);
        void SetCulture(string culture);
    }
} 