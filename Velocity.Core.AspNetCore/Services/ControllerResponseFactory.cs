using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Net;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ControllerResponseFactory
    {
        public static IMvcBuilder ConfigureInvalidModelStateResponse(this IMvcBuilder builder)
        {
            builder.ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = InvalidModelStateResponse);

            return builder;
        }

        public static Func<ActionContext, IActionResult> InvalidModelStateResponse => action =>
        {
            return new BadRequestObjectResult(new
            {
                Errors = new SerializableError(action.ModelState),
                Message = GetMessage(action.ModelState, HttpStatusCode.BadRequest)
            });
        };

        public static string GetMessage(ModelStateDictionary modelState, HttpStatusCode code)
        {
            var errors = string.Join(
                "\n",
                modelState.SelectMany(
                    m =>
                        m.Value.Errors.Select(
                            e =>
                                e.ErrorMessage
                            )
                    )
            );

            return string.IsNullOrWhiteSpace(errors) ? code.ToString() : errors;
        }
    }
}
