using Academix.Application.Common.Interfaces;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace Academix.Infrastructure.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IStringLocalizer _localizer;
        private string _currentCulture = "en";

        public LocalizationService(IStringLocalizer<LocalizationService> localizer)
        {
            _localizer = localizer;
        }

        public string GetCurrentCulture()
        {
            return _currentCulture;
        }

        public string GetLocalizedString(string key)
        {
            return _localizer[key];
        }

        public string GetLocalizedString(string key, params object[] args)
        {
            return _localizer[key, args];
        }

        public void SetCulture(string culture)
        {
            if (culture == "ar" || culture == "en")
            {
                _currentCulture = culture;
                var cultureInfo = new CultureInfo(culture);
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
            }
        }
    }
} 