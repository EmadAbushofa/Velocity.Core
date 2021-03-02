using System;
using System.Globalization;

namespace Velocity.Core.Extensions
{
    public static class NumbersExtensions
    {
        public static string ToDecimalString(this decimal value, string format = "0.000") => value.ToString(format).Replace(", ", ".").Replace("٫", ".");
        public static int ToInt32(this decimal value, IntPart part = IntPart.First)
        {
            var number = value.ToDecimalString();

            return part switch
            {
                IntPart.First => int.Parse(number.Split('.')[0]),
                IntPart.Second => int.Parse(number.Split('.')[1]),
                _ => throw new ArgumentOutOfRangeException(nameof(part), part, null),
            };
        }

        public static decimal ToRoundedDecimal(this decimal value)
        {
            var first = value.ToInt32();

            var second = value.ToInt32(IntPart.Second);

            if (second == 0 || second == 250 || second == 500 || second == 750)
                return decimal.Parse(first + "." + second, CultureInfo.InvariantCulture);

            if (second > 0 && second < 250)
                return decimal.Parse(first + ".250", CultureInfo.InvariantCulture);
            if (second > 250 && second < 500)
                return decimal.Parse(first + ".500", CultureInfo.InvariantCulture);
            if (second > 500 && second < 750)
                return decimal.Parse(first + ".750", CultureInfo.InvariantCulture);

            return decimal.Parse((first + 1).ToString(), CultureInfo.InvariantCulture);
        }

        public static string ToYearlyNumber(this long number, DateTime date)
            => date.Year + "-" + number;
        public static string ToYearlyNumber(this long number, string date)
            => date.ToDateTime().Year + "-" + number;

        public static long ToPureNumber(this string number)
        {
            var insideNumber = number.Contains("-") ? number.Split('-')[1] : number;

            long.TryParse(insideNumber.Trim(), out var newNumber);
            return newNumber;
        }

        public static string ToLyd(this decimal value) => ToMoney(value);
        public static string ToUsd(this decimal value) => ToMoney(value, decimalParts: 2);

        public static string ToMoney(this decimal value, int decimalParts = 3)
        {
            var str = new string('0', decimalParts);

            return value.ToString($"##,##0.{str}").Trim('.');
        }

        public static string ToMoney(this decimal? value, int decimalParts = 3)
        {
            var str = new string('0', decimalParts);

            return value?.ToString($"##,##0.{str}").Trim('.');
        }
    }
}