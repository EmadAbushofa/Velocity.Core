using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Velocity.Core.Extensions
{
    public static class HttpExtensions
    {
        public static async Task<Response<T>> GetJsonAsync<T>(this HttpClient httpClient, string uri, object query = null)
        {
            uri = query == null
                ? uri
                : $"{uri}?{GetQueryString(query)}";

            HttpResponseMessage message;

            try
            {
                message = await httpClient.GetAsync(uri);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Response.Fail(HttpStatusCode.ServiceUnavailable);
            }

            return await HttpResponse<T>.GetResponseAsync(message);
        }

        public static async Task<Response<T>> PostJsonAsync<T>(this HttpClient httpClient, string uri, object obj)
        {
            var input = obj.Serialize();

            HttpResponseMessage message;

            try
            {
                message = await httpClient.PostAsync(uri, new StringContent(input, Encoding.UTF8, "application/json"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Response.Fail(HttpStatusCode.ServiceUnavailable);
            }

            return await HttpResponse<T>.GetResponseAsync(message);
        }

        public static async Task<Response<T>> PutJsonAsync<T>(this HttpClient httpClient, string uri, object obj)
        {
            var input = obj.Serialize();

            HttpResponseMessage message;

            try
            {
                message = await httpClient.PutAsync(uri, new StringContent(input, Encoding.UTF8, "application/json"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Response.Fail(HttpStatusCode.ServiceUnavailable);
            }

            return await HttpResponse<T>.GetResponseAsync(message);
        }

        public static async Task<Response> DeleteJsonAsync(this HttpClient httpClient, string uri)
        {
            HttpResponseMessage message;

            try
            {
                message = await httpClient.DeleteAsync(uri);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Response.Fail(HttpStatusCode.ServiceUnavailable);
            }

            return (Response<object>)await HttpResponse<object>.GetResponseAsync(message);
        }

        public static async Task<Response<T>> PatchJsonAsync<T>(this HttpClient httpClient, string uri, object input = null)
        {
            HttpResponseMessage message;

            try
            {
                message = await httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), uri)
                {
                    Content = input == null
                        ? null
                        : new StringContent(input.Serialize(), Encoding.UTF8, "application/json")
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Response.Fail(HttpStatusCode.ServiceUnavailable);
            }

            return await HttpResponse<T>.GetResponseAsync(message);
        }


        public static string GetQueryString(this object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select FirstCharacterToLower(p.Name) + "=" + UrlEncode(p, obj);

            return string.Join("&", properties.ToArray());
        }

        public static string UrlEncode(PropertyInfo p, object obj)
        {
            var value = p.GetValue(obj, null);

            return value is DateTime dateTime
                ? HttpUtility.UrlEncode(dateTime.ToDateString())
                : HttpUtility.UrlEncode(value.ToString());
        }

        private static string FirstCharacterToLower(string str)
        {
            if (string.IsNullOrEmpty(str) || char.IsLower(str, 0))
                return str;

            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }


        public class HttpResponse<TModel>
        {
            public static async Task<HttpResponse<TModel>> GetResponseAsync(HttpResponseMessage message)
            {
                Console.WriteLine(message);

                var contentString = await message.Content.ReadAsStringAsync();

                Console.WriteLine(contentString);

                var response = contentString.DeserializeOrDefault<HttpResponse<TModel>>()
                         ?? new HttpResponse<TModel>();

                Console.WriteLine(response);

                return response;
            }

            public static implicit operator Response<TModel>(HttpResponse<TModel> httpResponse)
            {
                if ((int)httpResponse.StatusCode >= 200 && (int)httpResponse.StatusCode < 300)
                    return Response.Success(httpResponse.StatusCode, httpResponse.Message);

                return httpResponse.Errors.Count > 0
                    ? Response.Fail(httpResponse.StatusCode, httpResponse.Errors)
                    : Response.Fail(httpResponse.StatusCode, httpResponse.Message);
            }


            public Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();

            public string Message { get; set; }

            public HttpStatusCode StatusCode { get; set; }

            public TModel Result { get; set; }

            public HttpResponse<TOther> ToOtherModel<TOther>(Func<TModel, TOther> func)
            {
                return new HttpResponse<TOther>()
                {
                    Errors = Errors,
                    Message = Message,
                    Result = func(Result),
                    StatusCode = StatusCode,
                };
            }
        }
    }
}
