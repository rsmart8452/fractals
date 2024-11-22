namespace FractalFlair.Models;

public class Pixel
{
  public Pixel(int x, int y)
  {
    X = x;
    Y = y;
  }

  /// <summary>
  ///   Represent the pixel's X location within a bitmap
  /// </summary>
  public int X { get; }

  /// <summary>
  ///   Represents the pixels's Y location within a bitmap
  /// </summary>
  public int Y { get; }
}