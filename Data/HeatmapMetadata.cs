namespace datathingies.Data
{
    public record HeatmapMetadata
    {
        public double MinValue { get; init; } = 0;
        public double MaxValue { get; init; } = 0;
        public double MinWeeklyValue { get; init; } = 0;
        public double MaxWeeklyValue { get; init; } = 0;
        public ColorGradient Gradient { get; init; }
    }
}
