using System.Drawing;

namespace FractalFlair.Models;

public class ImageCoordinate
{
  public ImageCoordinate(Pixel pixel, BigComplex point)
  {
    Pixel = pixel;
    Point = point;
    Iterations = -1;
    Color = Color.Transparent;
  }

  public Pixel Pixel { get; }

  public Color Color { get; set; }

  /// <summary>
  ///   Complex number that represents the coordinate to calculate iterations against
  /// </summary>
  public BigComplex Point { get; }

  /// <summary>
  ///   Number of iterations that were calculated for the coordinate. Will be -1 if it hasn't been calculated yet.
  /// </summary>
  public int Iterations { get; set; }
}