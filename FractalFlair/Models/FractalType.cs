using FractalFlair.Models.PixelCalculators;
using System;
using System.Collections.Generic;

namespace FractalFlair.Models;

public static class FractalType
{
  public const string Mandelbrot = "Mandelbrot";
  public const string Julia = "Julia";
  public const string BurningShip = "Burning Ship";
  public const string Warped = "Warped";
  public const string CompressedFlamesPixel = "Compressed flames";

  public static IEnumerable<string> AllTypes => new[] { Mandelbrot, Julia, BurningShip, Warped, CompressedFlamesPixel };

  public static IPixelCalculator GetCalculator(string type)
  {
    return type switch
    {
      Mandelbrot => new MandelbrotPixel(),
      Julia => new JuliaPixel(),
      BurningShip => new BurningShipPixel(),
      Warped => new WarpedPixel(),
      CompressedFlamesPixel => new CompressedFlamesPixel(),
      _ => throw new ArgumentOutOfRangeException(nameof(type), $"Fractal type \"{type}\" not supported.")
    };
  }
}