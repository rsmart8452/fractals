using Singulink.Numerics;
using System;
using System.Numerics;

namespace FractalFlair.Models.PixelCalculators;

public class CompressedFlamesPixel : IPixelCalculator
{
  // These define the coordinates that completely contain the fractal's set when scale is 1
  private const double _upperLeftReal = -2d;
  private const double _upperLeftImaginary = -1.5d;
  private const double _lowerRightReal = 1d;
  private const double _lowerRightImaginary = 1.5d;
  private static readonly BigDecimal _two = 2;
  private static readonly BigDecimal _four = 4;


  public CompressedFlamesPixel()
  {
    StartingCenter = new BigComplex(-0.4d, -0.25d);
    StartingWidth = 3.25d;
    StartingHeight = 3.25d;
    Origin = new BigComplex(BigDecimal.Zero, BigDecimal.Zero);
  }

  /// <summary>
  ///   The initial center point that the fractal should focus on
  /// </summary>
  public BigComplex StartingCenter { get; }

  public BigComplex Origin { get; set; }

  /// <summary>
  ///   The width that shows the entire fractal set
  /// </summary>
  public double StartingWidth { get; }

  /// <summary>
  ///   The height that shows the entire fractal set
  /// </summary>
  public double StartingHeight { get; }

  public string Type => FractalType.CompressedFlamesPixel;

  public int Compute(Complex point, int maxIterations)
  {
    return Compute(point.Real, point.Imaginary, maxIterations);
  }

  public int Compute(BigComplex point, int maxIterations, bool useHighPrecision)
  {
    if (useHighPrecision)
      return Compute(point.Real, point.Imaginary, maxIterations);

    return Compute((double)point.Real, (double)point.Imaginary, maxIterations);
  }

  /// <summary>
  ///   Generates a color based on the x/y point as it would appear in the Mandelbrot algorithm.
  ///   By this I mean that you need to consider your Origin offset and scale (zoom) level in order
  ///   to work out what the actual X/Y is to generate a color for.
  /// </summary>
  /// <returns></returns>
  private static int Compute(double real, double imaginary, int maxIterations)
  {
    // Burning ship uses ABS values
    var realSquared = Math.Abs(real) * Math.Abs(real);
    var imaginarySquared = Math.Abs(imaginary) * Math.Abs(imaginary);
    var currentImaginary = 3d * Math.Abs(imaginary) * Math.Abs(real) + imaginary;
    var currentReal = realSquared - imaginarySquared + real;
    var iterations = 0;

    while (iterations < maxIterations && realSquared + imaginarySquared < 4d)
    {
      currentReal = Math.Abs(currentReal);
      //currentImaginary = Math.Abs(currentImaginary);
      realSquared = currentReal * currentReal;
      imaginarySquared = currentImaginary * currentImaginary;
      currentImaginary = 2d * currentImaginary * currentReal + imaginary;
      currentReal = realSquared - imaginarySquared + real;
      iterations++;
    }

    if (iterations > maxIterations)
      return int.MaxValue;

    return iterations;
  }

  /// <summary>
  ///   Traditional compute
  /// </summary>
  /// <returns></returns>
  private static int Compute(BigDecimal real, BigDecimal imaginary, int maxIterations)
  {
    // Burning ship uses ABS values
    var realSquared = BigDecimal.Abs(real) * BigDecimal.Abs(real);
    var imaginarySquared = BigDecimal.Abs(imaginary) * BigDecimal.Abs(imaginary);
    var currentY = _two * BigDecimal.Abs(imaginary) * BigDecimal.Abs(real) + imaginary;
    var currentX = realSquared - imaginarySquared + real;
    var iterations = 0;

    while (iterations < maxIterations && realSquared + imaginarySquared < _four)
    {
      currentX = BigDecimal.Abs(currentX);
      currentY = BigDecimal.Abs(currentY);
      realSquared = currentX * currentX;
      imaginarySquared = currentY * currentY;
      currentY = _two * currentY * currentX + imaginary;
      currentX = realSquared - imaginarySquared + real;
      iterations++;
    }

    if (iterations > maxIterations)
      return int.MaxValue;

    return iterations;
  }
}