using System;
using System.Text.RegularExpressions;

namespace datathingies
{
    public record Color
    {
        public float Red { get; init; }
        public float Green { get; init; }
        public float Blue { get; init; }

        public static Color Lerp(Color start, Color end, float amount)
            => new Color
            {
                Red = Lerp(start.Red, end.Red, amount),
                Green = Lerp(start.Green, end.Green, amount),
                Blue = Lerp(start.Blue, end.Blue, amount)
            };

        public static Color Lerp(Color start, Color middle, Color end, float amount)
            => new Color
            {
                Red = Lerp(start.Red, middle.Red, end.Red, amount),
                Green = Lerp(start.Green, middle.Green, end.Green, amount),
                Blue = Lerp(start.Blue, middle.Blue, end.Blue, amount)
            };

        public static float Lerp(float start, float end, float amount)
            => (1f - amount) * start + amount * end;

        public static float Lerp(float start, float middle, float end, float amount)
            => amount <= 0.5f ? Lerp(start, middle, amount * 2f) : Lerp(middle, end, (amount - .5f) * 2f);
    }

    public static class ColorExtensionMethods
    {
        public static Color ToColor(this string input)
        {
            if (!Regex.IsMatch(input, @"^#?[\da-fA-F]{6}$"))
                throw new ArgumentException($"{input} is not a valid hex color.", nameof(input));

            var temp = input.Replace("#", "");
            var r = (float)int.Parse(temp.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var g = (float)int.Parse(temp.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b = (float)int.Parse(temp.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            return new Color { Red = r / 255, Green = g / 255, Blue = b / 255 };
        }

        public static string ToHexString(this Color color)
        {
            var r = ((int)(color.Red * 255)).ToString("X2");
            var g = ((int)(color.Green * 255)).ToString("X2");
            var b = ((int)(color.Blue * 255)).ToString("X2");
            return $"#{r}{g}{b}";
        }

        public static Color LerpWith(this double value, double max = 100)
            => Color.Lerp(
                start: @"#63BE7B".ToColor(),
                middle: @"#FFEB84".ToColor(),
                end: @"#F8696B".ToColor(),
                amount: (float)(value / max));
    }
}
