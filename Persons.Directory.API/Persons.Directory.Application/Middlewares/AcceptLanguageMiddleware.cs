using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Persons.Directory.Application.Middlewares;

public class AcceptLanguageMiddleware
{
    private readonly RequestDelegate _next;

    public AcceptLanguageMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IOptions<RequestLocalizationOptions> options)
    {
        var supportedCultures = options.Value.SupportedCultures;
        var defaultCulture = options.Value.DefaultRequestCulture.Culture;
        var userLanguages = context.Request.Headers["Accept-Language"].ToString().Split(',');

        CultureInfo culture = defaultCulture;

        if (userLanguages.Length > 0)
        {
            foreach (var userLanguage in userLanguages)
            {
                try
                {
                    var cultureInfo = CultureInfo.GetCultureInfo(userLanguage.Trim());
                    if (supportedCultures.Contains(cultureInfo))
                    {
                        culture = cultureInfo;
                        break;
                    }
                }
                catch (CultureNotFoundException)
                {
                    // Ignore invalid culture names
                }
            }
        }

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        await _next(context);
    }

}
