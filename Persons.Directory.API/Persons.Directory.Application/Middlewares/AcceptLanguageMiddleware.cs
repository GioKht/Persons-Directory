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

        var culture = CultureInfo.DefaultThreadCurrentCulture;
        if (userLanguages.Length > 0)
        {
            var cultureName = userLanguages
                .Select(x => CultureInfo.GetCultureInfo(x.Trim()))
                .Where(x => supportedCultures.Contains(x))
                .FirstOrDefault()?.Name;

            if (!string.IsNullOrEmpty(cultureName))
            {
                culture = new CultureInfo(cultureName);
            }
        }

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        await _next(context);
    }
}
