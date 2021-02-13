using System;
using System.Text.RegularExpressions;

namespace datathingies
{
    public record Color(float red, float green, float blue)
    {
        public Color White { get { return new Color(1, 1, 1); } }
        public Color Black { get { return new Color(0, 0, 0); } }
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

            return new Color(r / 255, g / 255, b / 255);
        }

        public static string ToHexString(this Color color)
        {
            var r = (color.red * 255).ToString("X2");
            var g = (color.green * 255).ToString("X2");
            var b = (color.blue * 255).ToString("X2");
            return $"#{r}{g}{b}";
        }
    }
}
