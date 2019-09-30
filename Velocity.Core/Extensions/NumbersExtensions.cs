using System;

namespace Velocity.Core.Extensions
{
    public static class NumbersExtensions
    {
        public static int ToInt32(this decimal value, IntPart part = IntPart.First)
        {
            var number = value.ToString("0.000");

            switch (part)
            {
                case IntPart.First:
                    return int.Parse(number.Split('.')[0]);
                case IntPart.Second:
                    return int.Parse(number.Split('.')[1]);
                default:
                    throw new ArgumentOutOfRangeException(nameof(part), part, null);
            }
        }

        public static decimal ToRoundedDecimal(this decimal value)
        {
            var first = value.ToInt32();

            var second = value.ToInt32(IntPart.Second);

            if (second == 0 || second == 250 || second == 500 || second == 750)
                return decimal.Parse(first + "." + second);

            if (second > 0 && second < 250)
                return decimal.Parse(first + ".250");
            if (second > 250 && second < 500)
                return decimal.Parse(first + ".500");
            if (second > 500 && second < 750)
                return decimal.Parse(first + ".750");

            return decimal.Parse((first + 1).ToString());
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

        public static string ToLyd(this decimal value)
        {
            return value.ToString("##,##0.00");
        }

        public static string ToLyd(this decimal? value)
        {
            return value?.ToString("##,##0.00");
        }
    }
}