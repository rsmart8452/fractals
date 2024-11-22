using JetBrains.Annotations;
using Singulink.Numerics;
using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FractalFlair.Models;

[UsedImplicitly]
public class Fractal : IDisposable
{
  [UsedImplicitly]
  private Fractal()
  {
    Bitmap = new Bitmap(1, 1);
  }

  private Fractal(string type, int width, int height, BigComplex scale, BigComplex origin, int maxIterations)
  {
    Type = type;
    Width = width;
    Height = height;
    MaxIterations = maxIterations;
    Scale = scale;
    Origin = origin;
    Bitmap = new Bitmap(width, height);
    ImageCoordinates = new ImageCoordinate[width][];
    for (var x = 0; x < width; x++)
      ImageCoordinates[x] = new ImageCoordinate[height];
  }

  public int Width { get; }
  public int Height { get; }

  /// <summary>
  ///   The max iterations that the <see cref="IPixelCalculator" /> used (or will use) to calculate this fractal.
  /// </summary>
  public int MaxIterations { get; }

  public string Type { get; } = string.Empty;

  /// <summary>
  ///   How much the complex numbers change from pixel to pixel. Used to pre-populate ImageCoordinates
  ///   with accurate complex numbers given the image size.
  /// </summary>
  public BigComplex Scale { get; } = new(BigDecimal.Zero, BigDecimal.Zero);

  public BigComplex Origin { get; } = new(BigDecimal.Zero, BigDecimal.Zero);

  public double ZoomFactor { get; set; } = 1d;

  public Bitmap Bitmap { get; set; }

  // Jagged arrays perform better when indexing into them than 2-dimensional arrays.
  // They can also be paralleled easily, but 2-dimensional arrays don't work with Parallel.ForEach().
  public ImageCoordinate[][] ImageCoordinates { get; private set; } = Array.Empty<ImageCoordinate[]>();

  public static Task<Fractal> BuildFractalAsync(string type, int width, int height, BigComplex upperLeft, BigComplex scale, BigComplex origin, int maxIterations, IProgress<double> progress, CancellationToken token)
  {
    return Task.Run(() => BuildFractal(type, width, height, upperLeft, scale, origin, maxIterations, progress, token), token);
  }

  public BigComplex FindPoint(double xRatio, double yRatio)
  {
    return FindPoint((BigDecimal)xRatio, (BigDecimal)yRatio);
  }

  public BigComplex FindPoint(BigDecimal xRatio, BigDecimal yRatio)
  {
    var upperLeft = ImageCoordinates.First().First();
    var lowerRight = ImageCoordinates.Last().Last();
    var real = (lowerRight.Point.Real - upperLeft.Point.Real) * xRatio + upperLeft.Point.Real;
    var imaginary = (lowerRight.Point.Imaginary - upperLeft.Point.Imaginary) * yRatio + upperLeft.Point.Imaginary;
    return new BigComplex(real, imaginary);
  }

  private static Fractal BuildFractal(string type, int width, int height, BigComplex upperLeft, BigComplex scale, BigComplex origin, int maxIterations, IProgress<double> progress, CancellationToken token)
  {
    var fractal = new Fractal(type, width, height, scale, origin, maxIterations);
    var maxDimension = Math.Max(fractal.Width, fractal.Height);
    BigDecimal upperLeftReal;
    BigDecimal upperLeftImaginary;
    if (maxDimension > fractal.Width)
    {
      var widthPercentToSkip = (BigDecimal.One - fractal.Width / (BigDecimal)maxDimension) / 2;
      var widthPixelsToSkip = maxDimension * widthPercentToSkip;
      upperLeftReal = upperLeft.Real + fractal.Scale.Real * widthPixelsToSkip;
    }
    else
      upperLeftReal = upperLeft.Real;

    if (maxDimension > fractal.Height)
    {
      var heightPercentToSkip = (BigDecimal.One - fractal.Height / (BigDecimal)maxDimension) / 2;
      var heightPixelsToSkip = maxDimension * heightPercentToSkip;
      upperLeftImaginary = upperLeft.Imaginary + fractal.Scale.Imaginary * heightPixelsToSkip;
    }
    else
      upperLeftImaginary = upperLeft.Imaginary;

    var actualUpperLeft = new BigComplex(upperLeftReal, upperLeftImaginary);
    var completed = 0;

    for (var y = 0; y < fractal.Height; y++)
    {
      var imaginary = y * fractal.Scale.Imaginary + actualUpperLeft.Imaginary;
      token.ThrowIfCancellationRequested();
      for (var x = 0; x < fractal.Width; x++)
      {
        var pixel = new Pixel(x, y);
        var real = x * fractal.Scale.Real + actualUpperLeft.Real;
        fractal.ImageCoordinates[x][y] = new ImageCoordinate(pixel, new BigComplex(real, imaginary));
      }

      var completedCount = Interlocked.Increment(ref completed);
      progress.Report(completedCount / (double)fractal.Height);
    }

    return fractal;
  }

  #region Dispose

  private bool _disposed;

  protected virtual void Dispose([UsedImplicitly] bool disposeManagedResources)
  {
    if (_disposed)
      return;

    // Dispose unmanaged resources
    Bitmap.Dispose();

    // Release large resources - free up memory
    ImageCoordinates = Array.Empty<ImageCoordinate[]>();

    _disposed = true;
  }

  public void Dispose()
  {
    if (_disposed)
      return;

    Dispose(true);
    GC.SuppressFinalize(this);
  }

  ~Fractal()
  {
    Dispose(false);
  }

  #endregion
}