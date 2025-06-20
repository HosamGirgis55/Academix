using Academix.Application.Common.Interfaces;
using System.Globalization;

namespace Academix.WebAPI.Common.Middleware
{
    public class LocalizationMiddleware
    {
        private readonly RequestDelegate _next;

        public LocalizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILocalizationService localizationService)
        {
            var culture = ExtractCultureFromRequest(context);
            
            // Set the culture for the current request
            SetCulture(culture);
            localizationService.SetCulture(culture);
            
            // Add culture to HttpContext items for easy access
            context.Items["Culture"] = culture;
            
            await _next(context);
        }

        private string ExtractCultureFromRequest(HttpContext context)
        {
            // Check Accept-Language header
            var acceptLanguage = context.Request.Headers["Accept-Language"].FirstOrDefault();
            
            if (!string.IsNullOrEmpty(acceptLanguage))
            {
                // Parse Accept-Language header (e.g., "ar,en-US;q=0.9,en;q=0.8")
                var cultures = acceptLanguage.Split(',')
                    .Select(x => x.Split(';')[0].Trim())
                    .ToList();

                foreach (var culture in cultures)
                {
                    if (culture.StartsWith("ar"))
                        return "ar";
                    if (culture.StartsWith("en"))
                        return "en";
                }
            }

            // Check query parameter
            var queryLang = context.Request.Query["lang"].FirstOrDefault();
            if (!string.IsNullOrEmpty(queryLang))
            {
                if (queryLang == "ar" || queryLang == "en")
                    return queryLang;
            }

            // Default to English
            return "en";
        }

        private void SetCulture(string culture)
        {
            var cultureInfo = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
        }
    }

    public static class LocalizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseLocalization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LocalizationMiddleware>();
        }
    }
} 