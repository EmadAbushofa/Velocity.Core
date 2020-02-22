using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections;
using System.Net;
using Velocity.Core.Extensions;

namespace Velocity.Core.AspNetCore.Services
{
    public class UriHelper
    {
        private readonly NavigationManager _navigationManager;

        public UriHelper(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public virtual TModel GetFromQuery<TModel>()
            where TModel : new()
        {
            var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);

            var obj = new TModel();
            var properties = typeof(TModel).GetProperties();
            foreach (var property in properties)
            {
                if (!property.CanWrite || !property.CanRead)
                    continue;

                QueryHelpers.ParseQuery(uri.Query).TryGetValue(property.Name, out var param);
                var str = param.ToString();

                if (string.IsNullOrWhiteSpace(str))
                    continue;

                if (typeof(IList).IsAssignableFrom(property.PropertyType))
                {
                    var list = (IList)Activator.CreateInstance(property.PropertyType);
                    property.SetValue(obj, list);

                    foreach (var item in param)
                    {
                        var typeOfItem = property.PropertyType.GetGenericArguments()[0];

                        var val = typeOfItem.IsEnum
                            ? Enum.Parse(typeOfItem, item)
                            : Convert.ChangeType(item, typeOfItem);

                        list.Add(val);
                    }
                }
                else
                {
                    var t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    var newValue = t.IsEnum
                        ? Enum.Parse(t, str)
                        : Convert.ChangeType(str, t);

                    property.SetValue(obj, newValue);
                }
            }

            return obj;
        }

        public virtual void Navigate(object query)
        {
            var str = query.GetQueryString();

            var uriWithoutQuery = _navigationManager.Uri.Split('?')[0];

            _navigationManager.NavigateTo($"{uriWithoutQuery}?{str}");
        }

        public virtual void ClearQueryString()
        {
            var uriWithoutQuery = _navigationManager.Uri.Split('?')[0];

            _navigationManager.NavigateTo(uriWithoutQuery);
        }

        public virtual HttpResponse GetResponseFromQuery(string key)
        {
            var parts = _navigationManager.Uri.Split('?');
            var query = parts.Length > 1 ? parts[1] : null;

            if (string.IsNullOrWhiteSpace(query))
                return null;

            if (QueryHelpers.ParseNullableQuery(query).TryGetValue(key, out var value))
            {
                return HttpResponse.Success(HttpStatusCode.OK, value);
            }

            return null;
        }
    }
}
