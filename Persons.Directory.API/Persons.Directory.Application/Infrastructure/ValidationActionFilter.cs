using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persons.Directory.Application.Infrastructure
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
