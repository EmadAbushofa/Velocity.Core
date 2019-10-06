using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;

namespace Velocity.Core.Extensions
{
    public static class ReflectionExtensions
    {
        public static T CopyFrom<T>(this T obj1, T obj2)
        {
            foreach (var property in typeof(T).GetRuntimeProperties())
            {
                if (property.CanWrite)
                    obj1.SetValue(property.Name, property.GetValue(obj2));
            }

            return obj1;
        }

        public static T CopyTo<T>(this T obj1, T obj2)
        {
            foreach (var property in typeof(T).GetRuntimeProperties())
            {
                if (property.CanWrite)
                    obj2.SetValue(property.Name, property.GetValue(obj1));
            }

            return obj2;
        }

        [Obsolete]
        public static string DisplayName<TSource>(this TSource source, Expression<Func<TSource, object>> expression)
        {
            var name = expression?.ToExpressionTarget() ?? source.ToString();

            return source.GetType()
                       .GetRuntimeProperty(name)
                       ?.GetCustomAttribute<DisplayAttribute>()
                       ?.Name
                   ?? source.GetType()
                       .GetRuntimeField(name)
                       ?.GetCustomAttribute<DisplayAttribute>()
                       ?.Name;
        }

        [Obsolete]
        public static int? DisplayOrder<TSource>(this TSource source, Expression<Func<TSource, object>> expression)
        {
            var name = expression?.ToExpressionTarget() ?? source.ToString();

            return source.GetType()
                       .GetRuntimeProperty(name)
                       ?.GetCustomAttribute<DisplayAttribute>()
                       ?.Order
                   ?? source.GetType()
                       .GetRuntimeField(name)
                       ?.GetCustomAttribute<DisplayAttribute>()
                       ?.Order;
        }

        public static string DisplayName<TSource>(this TSource source)
        {
            var type = source.GetType();

            var property = type.GetRuntimeProperty(source.ToString());

            var display = property == null
                ? type.GetRuntimeField(source.ToString())
                    ?.GetCustomAttribute<DisplayAttribute>()
                : property.GetCustomAttribute<DisplayAttribute>();

            if (display == null || display.Name == null)
                return null;

            if (display.ResourceType == null)
                return display.Name;

            return new ResourceManager(display.ResourceType)?.GetString(display.Name)
                ?? display.Name;
        }
    }
}