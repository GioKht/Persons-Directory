using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;

namespace Persons.Directory.Application.Middlewares
{
    public class ValidationActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var culture = CultureInfo.CurrentUICulture;

                var response = new BadRequestResponse(string.Join(", ", errors));

                context.Result = new BadRequestObjectResult(new
                {
                    Error = response.Message
                });

                return;
            }

            await next();
        }
    }

    public class BadRequestResponse : IRequest<IActionResult>
    {
        public string Message { get; }

        public BadRequestResponse(string message)
        {
            Message = message;
        }
    }

}
