using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Velocity.Core.Extensions
{
    public static class SharedExtensions
    {
        public static string Ellipse(this string value, int max = 50)
            => value.Length <= max ? value : value.Substring(0, max) + "...";

        public static T ToEnum<T>(this string value) where T : struct
            => Enum.TryParse<T>(value, out var enumValue) ? enumValue : default;


        public static string ToExpressionTarget<TObject, TProperty>(
            this Expression<Func<TObject, TProperty>> expression)
            => ToExpressionString(expression);


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

        private static string ToExpressionString<T, TProperty>(Expression<Func<T, TProperty>> exp)
        {
            if (!TryFindMemberExpression(exp.Body, out var memberExp))
                return string.Empty;

            var memberNames = new Stack<string>();
            do
            {
                memberNames.Push(memberExp.Member.Name);
            }
            while (TryFindMemberExpression(memberExp.Expression, out memberExp));
            var members = memberNames.ToList();
            FilterFirstMember(members, exp);

            return string.Join(".", members);
        }

        private static void FilterFirstMember<T, TProperty>(List<string> members, Expression<Func<T, TProperty>> exp)
        {
            var expressionName = exp.Body.ToString();
            var parameterName = exp.Parameters.FirstOrDefault()?.Name;

            if (expressionName.Split('.')[0] != parameterName && members.Count > 1)
                members.RemoveAt(0);
        }

        // code adjusted to prevent horizontal overflow
        private static bool TryFindMemberExpression(Expression exp, out MemberExpression memberExp)
        {
            memberExp = exp as MemberExpression;
            if (memberExp != null)
            {
                // heyo! that was easy enough
                return true;
            }

            // if the compiler created an automatic conversion,
            // it'll look something like...
            // obj => Convert(obj.Property) [e.g., int -> object]
            // OR:
            // obj => ConvertChecked(obj.Property) [e.g., int -> long]
            // ...which are the cases checked in IsConversion
            if (!IsConversion(exp) || !(exp is UnaryExpression))
                return false;

            memberExp = ((UnaryExpression)exp).Operand as MemberExpression;
            return memberExp != null;
        }

        private static bool IsConversion(Expression exp)
            => exp?.NodeType == ExpressionType.Convert ||
               exp?.NodeType == ExpressionType.ConvertChecked;
    }
}
