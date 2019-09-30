using JetBrains.Annotations;
using System;
using System.Linq.Expressions;

namespace Velocity.Core.Extensions
{
    [PublicAPI]
    public static class SharedExtensions
    {
        public static string Ellipse(this string value, int max = 50)
            => value.Length <= max ? value : value.Substring(0, max) + "...";

        public static T ToEnum<T>(this string value) where T : struct
            => Enum.TryParse<T>(value, out var enumValue) ? enumValue : default(T);


        public static string ToExpressionTarget<TObject, TProperty>(
            this Expression<Func<TObject, TProperty>> expression)
            => TryConvert.ToExpressionString(expression);


        public static string ToFormattedString(this string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            if (value.Contains("{") && value.Contains("}"))
                return string.Format(value, parameterName);

            return value;
        }


        public static bool IsNullOrDefault<T>(this T argument)
        {
            if (argument is string)
                return string.IsNullOrWhiteSpace(argument.ToString());

            // deal with normal scenarios
            if (argument == null) return true;
            if (Equals(argument, default(T))) return true;

            // deal with non-null nullables
            var methodType = typeof(T);
            if (Nullable.GetUnderlyingType(methodType) != null) return false;

            // deal with boxed value types
            var argumentType = argument.GetType();
            if (argumentType.IsValueType && argumentType != methodType)
            {
                object obj = Activator.CreateInstance(argument.GetType());
                return obj.Equals(argument);
            }

            return false;
        }
    }
}