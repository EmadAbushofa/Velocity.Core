using System;
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
        public static async Task<HttpResponse<T>> GetJsonAsync<T>(this HttpClient httpClient, string uri, object query = null)
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
                return HttpResponse.Fail(HttpStatusCode.ServiceUnavailable);
            }

            return await GetResponseAsync<T>(message);
        }

        public static async Task<HttpResponse<T>> PostJsonAsync<T>(this HttpClient httpClient, string uri, object obj)
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
                return HttpResponse.Fail(HttpStatusCode.ServiceUnavailable);
            }

            return await GetResponseAsync<T>(message);
        }

        public static async Task<HttpResponse<T>> PutJsonAsync<T>(this HttpClient httpClient, string uri, object obj)
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
                return HttpResponse.Fail(HttpStatusCode.ServiceUnavailable);
            }

            return await GetResponseAsync<T>(message);
        }

        public static async Task<HttpResponse> DeleteJsonAsync(this HttpClient httpClient, string uri)
        {
            HttpResponseMessage message;

            try
            {
                message = await httpClient.DeleteAsync(uri);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return HttpResponse.Fail(HttpStatusCode.ServiceUnavailable);
            }

            return await GetResponseAsync<object>(message);
        }

        public static async Task<HttpResponse<T>> PatchJsonAsync<T>(this HttpClient httpClient, string uri, object input = null)
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
                return HttpResponse.Fail(HttpStatusCode.ServiceUnavailable);
            }

            return await GetResponseAsync<T>(message);
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


        public static async Task<HttpResponse<T>> GetResponseAsync<T>(HttpResponseMessage message)
        {
            Console.WriteLine(message);

            if (message.StatusCode == HttpStatusCode.InternalServerError)
                throw new ServerErrorException(message.ReasonPhrase);

            var contentString = await message.Content.ReadAsStringAsync();

            Console.WriteLine(contentString);

            var response = contentString.DeserializeOrDefault<HttpResponse<T>>();

            if (response != null)
            {
                response.StatusCode = message.StatusCode;
                return response;
            }

            return new HttpResponse<T>()
            {
                StatusCode = message.StatusCode,
                Message = message.ReasonPhrase,
            };
        }
    }
}
