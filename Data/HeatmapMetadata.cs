using System;

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

    public static class HeatmapMetadataExtensions
    {
        public static string GetColorAtAsHex(this HeatmapMetadata meta, double? value = 100)
        {
            if (value == null
                || value < meta.MinValue
                || value > meta.MaxValue)
                return "";

            return meta.Gradient
                .GetColorAt((value ?? 0).Remap(meta.MinValue, meta.MaxValue, 0, 1))
                .ToHexString();
        }

        public static double Remap(this double value, double from1, double to1, double from2, double to2)
            => (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
