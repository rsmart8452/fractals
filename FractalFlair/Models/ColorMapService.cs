using System;
using System.Drawing;
using System.Threading;

namespace FractalFlair.Models;

public class ColorMapService
{
  /// <summary>
  /// </summary>
  /// <param name="map">
  ///   If null, then uses color data generated with the fractal, otherwise applies the given map
  /// </param>
  /// <param name="fractal"></param>
  /// <param name="progress"></param>
  /// <param name="token"></param>
  /// <returns></returns>
  public Bitmap GenerateBitmapFromFractal(ColorMap? map, Fractal fractal, IProgress<FractalProgress> progress, CancellationToken token)
  {
    var bitmap = new Bitmap(fractal.Width, fractal.Height);
    var colorMap = map?.Clone();

    for (var x = 0; x < fractal.Width; x++)
    {
      token.ThrowIfCancellationRequested();
      for (var y = 0; y < fractal.Height; y++)
      {
        var coordinate = fractal.ImageCoordinates[x][y];
        if (colorMap != null)
          coordinate.Color = GetColorForIteration(coordinate.Iterations, fractal.MaxIterations, colorMap);

        bitmap.SetPixel(x, y, coordinate.Color);
      }

      var pct = (x + 1d) / (fractal.Width + 1d);
      progress.Report(new FractalProgress(1d, 1d, pct));
    }

    return bitmap;
  }

  /// <summary>
  ///   The actual color of a particular point is actually based on the iteration.
  ///   The iteration is how deep into the fractal function call we could go
  ///   before breaking the algorithm boundary. For example, if we managed to call
  ///   f(f(f(f(n)))) then the iteration was 4. The number of iterations that we
  ///   achieve is based on the actual X/Y defined, which is calculated prior to
  ///   this function call.
  /// </summary>
  public Color GetColorForIteration(int iteration, int maxIterations, ColorMap map)
  {
    // -1 means the pixel's color was never calculated (i.e. fractal
    // calculation was cancelled), so set to transparent to indicate this.
    if (iteration == -1)
      return Color.Transparent;

    // The first color in the array is reserved for the color
    // of all points that fall outside of the closed disk
    // that fully encompasses the Mandelbrot set.
    if (iteration <= 1)
    {
      return map.Map.Length < 1
        ? Color.Blue
        : map.Map[0];
    }

    // The second color is reserved for points that are
    // members of the fractal set.
    if (iteration >= maxIterations)
    {
      return map.Map.Length < 2
        ? Color.Black
        : map.Map[1];
    }

    // All other colors are are used for "boundary points" that
    // are near the points inside the fractal set.
    if (map.Map.Length < 3)
      return Color.Red; // Fall back in case no color map is set.

    // We loop through the color map if the color map is not
    // big enough for the iteration requested.
    var length = map.Map.Length - 2;
    var mod = iteration % length + 2;
    var color = map.Map[mod];
    return color;
  }
}