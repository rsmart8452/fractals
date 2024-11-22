using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FractalFlair.Models;

public static class ColorMaps
{
  private const int _defaultGradations = 75;

  public static IEnumerable<ColorMap> GetAvailableMaps(int gradations = _defaultGradations)
  {
    var maps = new[]
    {
      OldSchool(),
      Default(gradations),
      Gradual(gradations),
      Purple(gradations),
      GoldSilver(gradations),
      Fall(gradations)
    };

    return maps.OrderBy(p => p.Name.ToUpperInvariant());
  }

  public static ColorMap GetMap(string name, int gradations = _defaultGradations)
  {
    var map = GetAvailableMaps(gradations)
      .FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.InvariantCultureIgnoreCase));

    return map ?? Default(gradations);
  }

  /// <summary>
  ///   Original 16-color (CGA/EGA) palette used by older fractal programs.
  /// </summary>
  public static ColorMap OldSchool()
  {
    return new ColorMap(
      "Old School",
      1,
      new[]
      {
        Color.FromArgb(0, 0, 170),
        Color.Black,
        Color.FromArgb(0, 0, 170),
        Color.FromArgb(0, 170, 0),
        Color.FromArgb(0, 170, 170),
        Color.FromArgb(170, 0, 0),
        Color.FromArgb(170, 0, 170),
        Color.FromArgb(170, 85, 0),
        Color.FromArgb(170, 170, 170),
        Color.FromArgb(85, 85, 85),
        Color.FromArgb(85, 85, 255),
        Color.FromArgb(85, 255, 85),
        Color.FromArgb(85, 255, 255),
        Color.FromArgb(255, 85, 85),
        Color.FromArgb(255, 85, 255),
        Color.FromArgb(255, 255, 85),
        Color.FromArgb(255, 255, 255)
      });
  }

  /// <summary>
  ///   A long color map with a variety of fades. Ideal
  ///   choice for some nice fractals (and is also the default)
  /// </summary>
  public static ColorMap Default(int gradations = _defaultGradations)
  {
    var colors = new[]
    {
      Color.Blue,
      Color.Black,
      Color.Purple,
      Color.AntiqueWhite,
      Color.Red,
      Color.Orange,
      Color.Yellow,
      Color.DarkGreen,
      Color.LightBlue
    };

    return new ColorMap("Default", gradations, Fade(colors, gradations));
  }

  public static ColorMap Gradual(int gradations = _defaultGradations)
  {
    var colors = new[]
    {
      Color.Blue,
      Color.Black,
      Color.Purple,
      Color.AntiqueWhite,
      Color.Red,
      Color.Orange,
      Color.Yellow,
      Color.DarkGreen,
      Color.LightBlue
    };

    return new ColorMap("Gradual", gradations, Fade(colors, gradations * 4));
  }

  public static ColorMap Purple(int gradations = _defaultGradations)
  {
    var colors = new[]
    {
      Color.Blue,
      Color.Black,
      Color.Purple,
      Color.DarkBlue
    };

    return new ColorMap("Purple", gradations, Fade(colors, gradations * 8));
  }

  public static ColorMap GoldSilver(int gradations = _defaultGradations)
  {
    var colors = new[]
    {
      Color.Blue,
      Color.Black,
      Color.Gold,
      Color.FromArgb(10, 10, 10),
      Color.Silver,
      Color.FromArgb(10, 10, 10)
    };

    return new ColorMap("Gold & Silver", gradations, Fade(colors, gradations));
  }

  public static ColorMap Fall(int gradations = _defaultGradations)
  {
    var colors = new[]
    {
      Color.Blue,
      Color.Black,
      Color.Red,
      Color.Yellow,
      Color.Brown,
      Color.Green
    };

    return new ColorMap("Fall", gradations, Fade(colors, gradations));
  }

  public static Color[] TwoTone(Color color1, Color color2, int gradations)
  {
    var colors = new List<Color>();

    var fadeIn = GenerateFadeMap(gradations, color1, color2);
    var fadeOut = GenerateFadeMap(gradations, color2, color1);

    colors.AddRange(fadeIn);
    colors.AddRange(fadeOut);

    return colors.ToArray();
  }

  public static Color[] Fade(Color[] colors, int gradations)
  {
    if (colors.Length == 0)
      return new[] { Color.Red };

    if (colors.Length <= 2)
    {
      var copy = new List<Color>(colors);
      return copy.ToArray();
    }

    var list = new List<Color>
    {
      colors[0],
      colors[1]
    };

    for (var i = 2; i < colors.Length; i++)
    {
      var start = colors[i];
      var end = i >= colors.Length - 1
        ? colors[2]
        : colors[i + 1];

      var fade = GenerateFadeMap(gradations, start, end);
      list.AddRange(fade);
    }

    return list.ToArray();
  }

  /// <summary>
  ///   Lets you create a color map with a set number of
  ///   elements, fading between 2 colors
  /// </summary>
  private static Color[] GenerateFadeMap(int numberElements, Color startColor, Color endColor)
  {
    if (numberElements <= 1) return new[] { startColor };

    var redStep = (endColor.R - startColor.R) / (double)numberElements;
    var greenStep = (endColor.G - startColor.G) / (double)numberElements;
    var blueStep = (endColor.B - startColor.B) / (double)numberElements;
    var alphaStep = (endColor.A - startColor.A) / (double)numberElements;

    var colors = new Color[numberElements];
    colors[0] = startColor;

    double lastA = startColor.A;
    double lastR = startColor.R;
    double lastG = startColor.G;
    double lastB = startColor.B;

    for (var i = 1; i < numberElements; i++)
    {
      lastA += alphaStep;
      lastR += redStep;
      lastG += greenStep;
      lastB += blueStep;

      var alpha = (int)Math.Round(lastA, 0, MidpointRounding.AwayFromZero);
      var red = (int)Math.Round(lastR, 0, MidpointRounding.AwayFromZero);
      var blue = (int)Math.Round(lastB, 0, MidpointRounding.AwayFromZero);
      var green = (int)Math.Round(lastG, 0, MidpointRounding.AwayFromZero);

      if (alpha < 0)
        alpha = 0;

      if (alpha > 255)
        alpha = 255;

      if (red < 0)
        red = 0;

      if (red > 255)
        red = 255;

      if (blue < 0)
        blue = 0;

      if (blue > 255)
        blue = 255;

      if (green < 0)
        green = 0;

      if (green > 255)
        green = 255;

      var color = Color.FromArgb(alpha, red, green, blue);
      colors[i] = color;
    }

    return colors;
  }
}