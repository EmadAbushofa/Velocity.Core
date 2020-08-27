using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using System.Net;

namespace Velocity.Core.AspNetCore.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public abstract class ApiController : ControllerBase
    {
        protected CreatedResult Created<T>(string uri, T value)
        {
            return base.Created(uri, new
            {
                Data = value,
                Message = "success"
            });
        }

        protected CreatedResult Created(string uri, object value, string message)
        {
            return base.Created(uri, new
            {
                Data = value,
                Message = message
            });
        }

        protected OkObjectResult Ok<T>(T value)
        {
            return base.Ok(new
            {
                Data = value,
                Message = "success"
            });
        }

        protected OkObjectResult Ok(string message)
        {
            return base.Ok(new
            {
                Message = message
            });
        }

        protected OkObjectResult Ok(object value, string message)
        {
            return base.Ok(new
            {
                Data = value,
                Message = message
            });
        }

        protected ConflictObjectResult Conflict<T>(T errors)
        {
            if (errors is ModelStateDictionary modelState)
                return base.Conflict(new
                {
                    Errors = new SerializableError(modelState),
                    Message = GetMessage(modelState, HttpStatusCode.Conflict)
                });

            return base.Conflict(new
            {
                Errors = errors,
            });
        }

        protected ConflictObjectResult Conflict(string message)
        {
            return base.Conflict(new
            {
                Message = message,
            });
        }

        protected BadRequestObjectResult BadRequest<T>(T errors)
        {
            if (errors is ModelStateDictionary modelState)
                return base.BadRequest(new
                {
                    Errors = new SerializableError(modelState),
                    Message = GetMessage(modelState, HttpStatusCode.BadRequest)
                });

            return base.BadRequest(new
            {
                Errors = errors,
            });
        }

        protected BadRequestObjectResult BadRequest(string message)
        {
            return base.BadRequest(new
            {
                Message = message,
            });
        }

        protected ActionResult GetResult(HttpResponse response)
        {
            return StatusCode((int)response.StatusCode, new
            {
                response.Message
            });
        }


        private string GetMessage(ModelStateDictionary modelState, HttpStatusCode code)
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