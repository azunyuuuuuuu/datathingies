using System;

namespace datathingies.Data
{
    public record Covid19WeeklyData
    {
        public string Week { get; init; }
        public string Month { get; init; }
        public double? Monday { get; init; }
        public double? Tuesday { get; init; }
        public double? Wednesday { get; init; }
        public double? Thursday { get; init; }
        public double? Friday { get; init; }
        public double? Saturday { get; init; }
        public double? Sunday { get; init; }
        public double? Weekly { get; init; }
        public string ColorMonday { get; init; }
        public string ColorTuesday { get; init; }
        public string ColorWednesday { get; init; }
        public string ColorThursday { get; init; }
        public string ColorFriday { get; init; }
        public string ColorSaturday { get; init; }
        public string ColorSunday { get; init; }
        public DateTime Date { get; internal set; }
    }
}
