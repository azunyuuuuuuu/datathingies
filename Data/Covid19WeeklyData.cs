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
        public Color ColorMonday { get; init; }
        public Color ColorTuesday { get; init; }
        public Color ColorWednesday { get; init; }
        public Color ColorThursday { get; init; }
        public Color ColorFriday { get; init; }
        public Color ColorSaturday { get; init; }
        public Color ColorSunday { get; init; }
    }
}
