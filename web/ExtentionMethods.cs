using System;
using System.Globalization;

namespace datathingies
{
    public static class ExtentionMethods
    {
        // https://stackoverflow.com/a/1497620
        internal static int WeekOfYearISO8601(this DateTime date)
        {
            var day = (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(date);
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date.AddDays(4 - (day == 0 ? 7 : day)), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        internal static string WeekYear(this DateTime date)
        {
            var weekofyear = date.WeekOfYearISO8601();
            var correctedyear = date.Month == 1 && weekofyear > 50 ? date.Year - 1 : date.Year;
            return $"{correctedyear.ToString("D4")} {weekofyear.ToString("D2")}";
        }
    }
}
