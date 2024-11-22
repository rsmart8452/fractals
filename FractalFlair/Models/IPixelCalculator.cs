using System.Numerics;

namespace FractalFlair.Models;

public interface IPixelCalculator
{
  public string Type { get; }

  /// <summary>
  ///   The initial center point that the fractal should focus on
  /// </summary>
  BigComplex StartingCenter { get; }

  /// <summary>
  ///   Specific to the Julia set, this origin can be changed to "animate" the set
  /// </summary>
  BigComplex Origin { get; set; }

  /// <summary>
  ///   The width that shows the entire fractal set
  /// </summary>
  double StartingWidth { get; }

  /// <summary>
  ///   The height that shows the entire fractal set
  /// </summary>
  double StartingHeight { get; }

  /// <summary>
  ///   Computes the number of iterations for the given point
  /// </summary>
  /// <param name="point">
  ///   Complex number to calculate against
  /// </param>
  /// <param name="maxIterations">
  ///   Number of iterations to use when trying to determine infinity
  /// </param>
  /// <returns>
  ///   The number of iterations calculated
  /// </returns>
  int Compute(Complex point, int maxIterations);

  /// <summary>
  ///   Computes the number of iterations for the given point
  /// </summary>
  /// <param name="point">
  ///   Complex number to calculate against
  /// </param>
  /// <param name="maxIterations">
  ///   Number of iterations to use when trying to determine infinity
  /// </param>
  /// <param name="useHighPrecision">
  /// </param>
  /// <returns>
  ///   The number of iterations calculated
  /// </returns>
  int Compute(BigComplex point, int maxIterations, bool useHighPrecision);
}