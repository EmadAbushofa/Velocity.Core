using System.Collections.Generic;
using System.Net;

namespace Velocity.Core
{
    public class HttpResponse
    {
        public static HttpResponse<T> Success<T>(T data)
            => new HttpResponse<T>(data, Success("Success"));

        public static HttpResponse<T> Success<T>(T data, string message)
            => new HttpResponse<T>(data, Success(message));

        public static HttpResponse<T> Success<T>(T data, HttpStatusCode statusCode, string message)
            => new HttpResponse<T>(data, Success(statusCode, message));

        public static HttpResponse Fail(string message)
            => new HttpResponse()
            {
                Message = message,
                StatusCode = HttpStatusCode.BadRequest,
            };

        public static HttpResponse Fail(Dictionary<string, List<string>> errors)
            => new HttpResponse()
            {
                Errors = errors,
                StatusCode = HttpStatusCode.BadRequest,
            };

        public static HttpResponse Success(string message)
            => new HttpResponse()
            {
                Message = message,
                StatusCode = HttpStatusCode.OK
            };

        public static HttpResponse Success(HttpStatusCode statusCode, string message)
            => new HttpResponse()
            {
                StatusCode = statusCode,
                Message = message
            };

        public static HttpResponse Fail(HttpStatusCode statusCode)
            => new HttpResponse()
            {
                StatusCode = statusCode,
            };

        public static HttpResponse Fail(HttpStatusCode statusCode, string message)
            => new HttpResponse()
            {
                StatusCode = statusCode,
                Message = message
            };

        public static HttpResponse Fail(HttpStatusCode statusCode, Dictionary<string, List<string>> errors)
            => new HttpResponse()
            {
                StatusCode = statusCode,
                Errors = errors
            };

        private HttpResponse() { }

        public static HttpResponse Convert<T>(HttpResponse<T> response)
            => new HttpResponse()
            {
                Message = response.Message,
                StatusCode = response.StatusCode,
                Errors = response.Errors,
            };

        public Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();
        public string Message { get; private set; }
        public bool IsValid => (int)StatusCode >= 200 && (int)StatusCode < 300;
        public HttpStatusCode StatusCode { get; private set; }
    }


    public class HttpResponse<T>
    {
        public HttpResponse()
        {

        }

        public HttpResponse(T data, HttpResponse response)
        {
            Data = data;
            Message = response.Message;
            Errors = response.Errors;
            StatusCode = response.StatusCode;
        }

        public static implicit operator HttpResponse(HttpResponse<T> response)
            => HttpResponse.Convert(response);

        public static implicit operator HttpResponse<T>(HttpResponse response)
            => Convert(response);

        public Dictionary<string, List<string>> Errors { get; internal set; } = new Dictionary<string, List<string>>();
        public string Message { get; set; }
        public bool IsValid => (int)StatusCode >= 200 && (int)StatusCode < 300;
        public HttpStatusCode StatusCode { get; set; }
        public T Data { get; set; }

        public static HttpResponse<T> Convert(HttpResponse response)
            => new HttpResponse<T>()
            {
                Message = response.Message,
                StatusCode = response.StatusCode,
                Errors = response.Errors
            };
    }
}
