namespace datathingies.Data
{
    public record Covid19WeeklyData
    {
        public string Week { get; init; }
        public string Month { get; init; }
        public Covid19DataEntry Monday { get; init; }
        public Covid19DataEntry Tuesday { get; init; }
        public Covid19DataEntry Wednesday { get; init; }
        public Covid19DataEntry Thursday { get; init; }
        public Covid19DataEntry Friday { get; init; }
        public Covid19DataEntry Saturday { get; init; }
        public Covid19DataEntry Sunday { get; init; }
    }
}
