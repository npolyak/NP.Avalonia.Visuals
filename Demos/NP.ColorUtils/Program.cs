using Avalonia;
using Avalonia.Media;
using NP.Avalonia.Visuals.ColorUtils;
using System;

namespace NP.ThemingPrototype
{
    class Program
    {
        public static void Main(string[] args)
        {
            Color color = Color.Parse("#302F4A"); // Color.Parse("#E64343");

            Console.WriteLine(color.ToStr());

            HslColor hslColor = color.ToHSL();

            Console.WriteLine(hslColor.ToString());

            Color restoredColor = hslColor.ToColor();

            Console.WriteLine(restoredColor.ToStr());
        }
    }
}
