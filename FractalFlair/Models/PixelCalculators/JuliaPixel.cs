using Singulink.Numerics;
using System.Numerics;

namespace FractalFlair.Models.PixelCalculators;

public class JuliaPixel : IPixelCalculator
{
  // These define the coordinates that completely contain the fractal's set when scale is 1
  private const double _upperLeftReal = -2d;
  private const double _upperLeftImaginary = -1.5d;
  private const double _lowerRightReal = 1d;
  private const double _lowerRightImaginary = 1.5d;
  private static readonly BigDecimal _two = 2;
  private static readonly BigDecimal _four = 4;


  public JuliaPixel()
  {
    StartingCenter = new BigComplex(BigDecimal.Zero, BigDecimal.Zero);
    StartingWidth = 4d;
    StartingHeight = 4d;
    Origin = new BigComplex(-0.7269d, 0.1889);
  }

  /// <summary>
  ///   Specific to the Julia set, this origin can be changed to "animate" the set
  /// </summary>
  public BigComplex Origin { get; set; }

  /// <summary>
  ///   The initial center point that the fractal should focus on
  /// </summary>
  public BigComplex StartingCenter { get; }

  /// <summary>
  ///   The width that shows the entire fractal set
  /// </summary>
  public double StartingWidth { get; }

  /// <summary>
  ///   The height that shows the entire fractal set
  /// </summary>
  public double StartingHeight { get; }

  public string Type => FractalType.Julia;

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
  ///   Traditional compute
  /// </summary>
  /// <returns></returns>
  private int Compute(double real, double imaginary, int maxIterations)
  {
    var originX = (double)Origin.Real;
    var originY = (double)Origin.Imaginary;
    var realSquared = real * real;
    var imaginarySquared = imaginary * imaginary;
    var currentY = imaginary;
    var currentX = real;
    var iterations = 0;

    while (iterations <= maxIterations && realSquared + imaginarySquared < 4d)
    {
      realSquared = currentX * currentX;
      imaginarySquared = currentY * currentY;
      currentY = 2d * currentY * currentX + originY;
      currentX = realSquared - imaginarySquared + originX;
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
  private int Compute(BigDecimal real, BigDecimal imaginary, int maxIterations)
  {
    var originX = Origin.Real;
    var originY = Origin.Imaginary;
    var realSquared = real * real;
    var imaginarySquared = imaginary * imaginary;
    var currentY = imaginary;
    var currentX = real;
    var iterations = 0;

    while (iterations < maxIterations && realSquared + imaginarySquared < _four)
    {
      realSquared = currentX * currentX;
      imaginarySquared = currentY * currentY;
      currentY = _two * currentY * currentX + originY;
      currentX = realSquared - imaginarySquared + originX;
      iterations++;
    }

    if (iterations > maxIterations)
      return int.MaxValue;

    return iterations;
  }
}