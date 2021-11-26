using Avalonia.Media;
using NP.Utilities;
using System;

namespace NP.Avalonia.Visuals.ColorUtils
{
    public static class ColorHelper
    {
        /// <summary>
        /// this is agorithm from https://stackoverflow.com/questions/39118528/rgb-to-hsl-conversion/39147465#39147465
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static HslColor ToHSL(this Color color)
        {
            int a = color.A;

            double r = color.R / 255d;
            double g = color.G / 255d;
            double b = color.B / 255d;

            double max = MathUtils.Max(r, g, b);
            double min = MathUtils.Min(r, g, b);

            double delta = max - min;

            double hue;
            if (delta.AlmostEquals(0d)) // some very small number since c is double
            {
                hue = 0;
            }
            else
            {
                double segment;
                double shift;
                if (max.AlmostEquals(r))
                {
                    segment = (g - b) / delta;
                    shift = segment < 0 ? 6 : 0;
                }
                else if (max.AlmostEquals(g))
                {
                    segment = (b - r) / delta;
                    shift = 2;
                }
                else if (max.AlmostEquals(b))
                {
                    segment = (r - g) / delta;
                    shift = 4;
                }
                else
                {
                    throw new ProgrammingError("Should never get here.");
                }
                hue = segment + shift;

                hue *= 60;
            }

            double sum = max + min;
            double lightness = sum / 2d * 100d;
            double saturation = delta / (1 - Math.Abs(1 - sum)) * 100d;

            return new HslColor(a, (float) hue, (float) saturation, (float) lightness);
        }
    }
}
