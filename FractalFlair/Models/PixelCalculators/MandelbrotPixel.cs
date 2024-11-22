using Singulink.Numerics;
using System.Numerics;

namespace FractalFlair.Models.PixelCalculators;

public class MandelbrotPixel : IPixelCalculator
{
  // These define the coordinates that completely contain the fractal's set when scale is 1
  private const double _upperLeftReal = -2d;
  private const double _upperLeftImaginary = -1.5d;
  private const double _lowerRightReal = 1d;
  private const double _lowerRightImaginary = 1.5d;
  private static readonly BigDecimal _two = 2;
  private static readonly BigDecimal _four = 4;


  public MandelbrotPixel()
  {
    StartingCenter = new BigComplex(-0.5d, 0d);
    StartingWidth = 3d;
    StartingHeight = 3d;
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

  public string Type => FractalType.Mandelbrot;

  public int Compute(Complex point, int maxIterations)
  {
    var iterations = 0;
    var z = point;
    while (Complex.Abs(z) < 2d && iterations < maxIterations)
    {
      z = z * z + point;
      iterations++;
    }

    if (iterations >= maxIterations)
      return int.MaxValue;

    return iterations;
  }

  public int Compute(BigComplex point, int maxIterations, bool useHighPrecision)
  {
    if (useHighPrecision)
      return Compute(point.Real, point.Imaginary, maxIterations);

    return Compute((double)point.Real, (double)point.Imaginary, maxIterations);
  }

  /// <summary>
  ///   Traditional compute
  /// </summary>
  /// <returns></returns>
  private static int Compute(double real, double imaginary, int maxIterations)
  {
    var iterations = 0;
    var currentRealSquared = real * real;
    var currentImaginarySquared = imaginary * imaginary;
    var currentImaginary = 2d * imaginary * real + imaginary;
    var currentReal = currentRealSquared - currentImaginarySquared + real;

    while (iterations <= maxIterations && currentRealSquared + currentImaginarySquared < 4d)
    {
      currentRealSquared = currentReal * currentReal;
      currentImaginarySquared = currentImaginary * currentImaginary;
      // Standard mandelbrot uses 2 here
      currentImaginary = 2d * currentImaginary * currentReal + imaginary;
      currentReal = currentRealSquared - currentImaginarySquared + real;
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
    var iterations = 0;
    var currentRealSquared = real * real;
    var currentImaginarySquared = imaginary * imaginary;
    var currentImaginary = _two * imaginary * real + imaginary;
    var currentReal = currentRealSquared - currentImaginarySquared + real;

    while (iterations <= maxIterations && currentRealSquared + currentImaginarySquared < _four)
    {
      currentRealSquared = currentReal * currentReal;
      currentImaginarySquared = currentImaginary * currentImaginary;
      // Standard mandelbrot uses 2 here
      currentImaginary = _two * currentImaginary * currentReal + imaginary;
      currentReal = currentRealSquared - currentImaginarySquared + real;
      iterations++;
    }

    if (iterations > maxIterations)
      return int.MaxValue;

    return iterations;
  }
}