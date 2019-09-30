using System.Collections.Generic;
using System.Net;

namespace Velocity.Core.Extensions
{
    public class Response
    {
        public static Response Success(HttpStatusCode statusCode)
            => new Response()
            {
                IsValid = true,
                StatusCode = statusCode,
            };

        public static Response Success(HttpStatusCode statusCode, string message)
            => new Response()
            {
                IsValid = true,
                StatusCode = statusCode,
                Message = message
            };

        public static Response Fail(HttpStatusCode statusCode)
            => new Response()
            {
                IsValid = false,
                StatusCode = statusCode,
            };

        public static Response Fail(HttpStatusCode statusCode, string message)
            => new Response()
            {
                IsValid = false,
                StatusCode = statusCode,
                Message = message
            };

        public static Response Fail(HttpStatusCode statusCode, Dictionary<string, List<string>> errors)
            => new Response()
            {
                IsValid = false,
                StatusCode = statusCode,
                Errors = errors
            };

        private Response() { }

        public static Response Convert<T>(Response<T> response)
            => new Response()
            {
                IsValid = response.IsValid,
                Message = response.Message,
                StatusCode = response.StatusCode,
                Errors = response.Errors
            };

        public Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();
        public string Message { get; private set; }
        public bool IsValid { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
    }


    public class Response<T>
    {
        public static implicit operator Response(Response<T> response)
            => Response.Convert(response);

        public static implicit operator Response<T>(Response response)
            => Convert(response);

        public static Response<T> Success(T model, HttpStatusCode statusCode)
            => new Response<T>()
            {
                IsValid = true,
                Model = model,
                StatusCode = statusCode,
            };

        public static Response<T> Success(T model, HttpStatusCode statusCode, string message)
            => new Response<T>()
            {
                IsValid = true,
                Model = model,
                StatusCode = statusCode,
                Message = message
            };

        public static Response<T> Fail(HttpStatusCode statusCode)
            => new Response<T>()
            {
                IsValid = false,
                Model = default,
                StatusCode = statusCode,
            };

        public static Response<T> Fail(HttpStatusCode statusCode, string message)
            => new Response<T>()
            {
                IsValid = false,
                Model = default,
                StatusCode = statusCode,
                Message = message
            };

        public static Response<T> Fail(HttpStatusCode statusCode, Dictionary<string, List<string>> errors)
            => new Response<T>()
            {
                IsValid = false,
                StatusCode = statusCode,
                Errors = errors
            };

        private Response() { }

        public Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();
        public string Message { get; private set; }
        public bool IsValid { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public T Model { get; private set; }

        public static Response<T> Convert(Response response)
            => new Response<T>()
            {
                IsValid = response.IsValid,
                Message = response.Message,
                StatusCode = response.StatusCode,
                Errors = response.Errors
            };
    }
}
