// Ignore Spelling: kvp

using FractalFlair.Models.PixelCalculators;
using Singulink.Numerics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FractalFlair.Models;

/// <summary>
///   Defines a fractal and stores the calculated result
/// </summary>
public class FractalCalculator
{
  private int _totalCompletedPixels;
  private readonly IPixelCalculator _calculator;
  private readonly ColorMapService _colorMapService = new();
  private readonly ColorMap _map;
  private readonly IProgress<FractalProgress> _progress;
  private readonly IProgress<double> _initializeProgress;

  /// <summary>
  ///   Defines a fractal
  /// </summary>
  /// <param name="calculator"></param>
  /// <param name="map"></param>
  /// <param name="progress"></param>
  public FractalCalculator(IPixelCalculator calculator, ColorMap map, IProgress<FractalProgress> progress)
  {
    _calculator = calculator;
    _map = map;
    _progress = progress;
    _initializeProgress = new Progress<double>(p => _progress.Report(new FractalProgress(p, 0d, 0d)));
  }

  /// <summary>
  ///   First calculation of a fractal using the defaults provided by the <see cref="IPixelCalculator" />
  /// </summary>
  /// <param name="width">
  ///   Bitmap width
  /// </param>
  /// <param name="height">
  ///   Bitmap height
  /// </param>
  /// <param name="maxIterations">
  ///   Maximum number of iterations to compute before deciding the point being computed will reach infinity
  /// </param>
  /// <param name="token">
  /// </param>
  /// <returns></returns>
  public async Task<Fractal> CalculateAsync(int width, int height, int maxIterations, CancellationToken token)
  {
    var center = _calculator.StartingCenter;
    var left = center.Real - (BigDecimal)(_calculator.StartingWidth / 2d);
    var upper = center.Imaginary - (BigDecimal)(_calculator.StartingHeight / 2d);
    var upperLeft = new BigComplex(left, upper);
    var right = center.Real + (BigDecimal)(_calculator.StartingWidth / 2d);
    var lower = center.Imaginary + (BigDecimal)(_calculator.StartingHeight / 2d);
    var lowerRight = new BigComplex(right, lower);
    var fractal = await InitializeFractalAsync(width, height, upperLeft, lowerRight, maxIterations, token);
    return await CalculateInitializedFractalAsync(fractal, maxIterations, false, token);
  }

  /// <summary>
  ///   Calculates the fractal from the zoomed in or zoomed out image data provided
  /// </summary>
  /// <param name="original">
  ///   Fractal that is being zoomed in on
  /// </param>
  /// <param name="width">
  ///   Bitmap width to use for the image that is being calculated
  /// </param>
  /// <param name="height">
  ///   Bitmap height to use for the image that is being calculated
  /// </param>
  /// <param name="centerRatioX">
  /// </param>
  /// <param name="centerRatioY">
  /// </param>
  /// <param name="zoomScale">
  ///   The scale of the zoom
  /// </param>
  /// <param name="maxIterations">
  ///   Maximum number of iterations to compute before deciding the point being computed will reach infinity
  /// </param>
  /// <param name="useHighPrecision"></param>
  /// <param name="token"></param>
  /// <returns></returns>
  public async Task<Fractal> CalculateAsync(Fractal original, int width, int height, double centerRatioX, double centerRatioY, double zoomScale, int maxIterations, bool useHighPrecision, CancellationToken token)
  {
    _calculator.Origin = original.Origin;
    var upperLeft = ZoomedUpperLeft(original, width, height, centerRatioX, centerRatioY, zoomScale);
    var fractal = await InitializeZoomedFractalAsync(original, width, height, upperLeft, zoomScale, maxIterations, token);
    return await CalculateInitializedFractalAsync(fractal, maxIterations, useHighPrecision, token);
  }

  public Task<Fractal> CalculateJuliaFromOriginAsync(Fractal original, int width, int height, double xRatio, double yRatio, int maxIterations, CancellationToken token)
  {
    var origin = original.FindPoint((BigDecimal)xRatio, (BigDecimal)yRatio);
    return CalculateJuliaFromOriginAsync(width, height, origin, maxIterations, token);
  }

  public Task<Fractal> CalculateJuliaFromOriginAsync(int width, int height, BigComplex origin, int maxIterations, CancellationToken token)
  {
    _calculator.Origin = origin;
    return CalculateAsync(width, height, maxIterations, token);
  }

  public async Task<Tuple<Fractal, BitmapImage>> AnimateJuliaAsync(int width, int height, BigComplex startOrigin, BigComplex endOrigin, int steps, int maxIterations, CancellationToken token)
  {
    var fractals = new List<Fractal>();
    var stepScaleReal = (endOrigin.Real - startOrigin.Real) / steps;
    var stepsScaleImaginary = (endOrigin.Imaginary - startOrigin.Imaginary) / steps;
    var progressList = new Dictionary<int, FractalProgress>();
    for (var i = 0; i < steps; i++)
    {
      var id = i;
      var originReal = startOrigin.Real + stepScaleReal * i;
      var originImaginary = startOrigin.Imaginary + stepsScaleImaginary * i;
      var pixelCalculator = new JuliaPixel
      {
        Origin = new BigComplex(originReal, originImaginary)
      };

      var calculatorProgress = new Progress<FractalProgress>(p => AnimateJuliaProgress(progressList, p, id, steps));
      var fractalCalculator = new FractalCalculator(pixelCalculator, _map, calculatorProgress);
      fractals.Add(await fractalCalculator.CalculateAsync(width, height, maxIterations, token));
    }

    var bitmaps = fractals.Select(f => f.Bitmap).ToArray();
    var svc = new AnimationService();

    var time = DateTime.Now;
    var fileName = $"JuliaAnimated.{time:yyyy-MM-dd_HH.mm.ss}.gif";
    var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "FractalFlair");
    if (!Directory.Exists(dir))
      Directory.CreateDirectory(dir);

    var filePath = Path.Combine(dir, fileName);
    var bitmapImage = await svc.AnimateBitmapsAsync(bitmaps, filePath);
    return new Tuple<Fractal, BitmapImage>(fractals.First(), bitmapImage);
  }

  private void AnimateJuliaProgress(IDictionary<int, FractalProgress> progressList, FractalProgress progress, int id, int totalSteps)
  {
    progressList[id] = progress;
    var initializePercent = progressList.Sum(kvp => kvp.Value.InitializePercent) / totalSteps;
    var calculatePercent = progressList.Sum(kvp => kvp.Value.CalculatePercent) / totalSteps;
    var colorPercent = progressList.Sum(kvp => kvp.Value.ColorMapPercent) / totalSteps;
    _progress.Report(new FractalProgress(initializePercent, calculatePercent, colorPercent));
  }

  private Task<Fractal> CalculateInitializedFractalAsync(Fractal fractal, int maxIterations, bool useHighPrecision, CancellationToken token)
  {
    return Task.Run(() => CalculateInitializedFractal(fractal, maxIterations, useHighPrecision, token), token);
  }

  private Fractal CalculateInitializedFractal(Fractal fractal, int maxIterations, bool useHighPrecision, CancellationToken token)
  {
    // Reset completed pixel count
    Interlocked.Exchange(ref _totalCompletedPixels, 0);
    var totalPixels = fractal.ImageCoordinates.Sum(s => s.Length);

    try
    {
      Parallel.ForEach(
        fractal.ImageCoordinates,
        new ParallelOptions { CancellationToken = token },
        c => CalculateStrip(c, maxIterations, totalPixels, useHighPrecision, token));

      var colorMapService = new ColorMapService();
      token.ThrowIfCancellationRequested();
      fractal.Bitmap = colorMapService.GenerateBitmapFromFractal(_map, fractal, _progress, token);
    }
    catch (Exception ex) when (ex.IsCancel())
    {
      // If user cancels, return what's been calculated so far
    }

    return fractal;
  }

  private async Task<Fractal> InitializeFractalAsync(int width, int height, BigComplex upperLeft, BigComplex lowerRight, int maxIterations, CancellationToken token)
  {
    var maxDimension = Math.Max(width, height) - 1;
    var scaleReal = (lowerRight.Real - upperLeft.Real) / maxDimension;
    var scaleImaginary = (lowerRight.Imaginary - upperLeft.Imaginary) / maxDimension;
    var scale = new BigComplex(scaleReal, scaleImaginary);
    var fractal = await FillFractalAsync(_calculator.Type, width, height, upperLeft, scale, _calculator.Origin, maxIterations, token);
    if (height == width)
      return fractal;

    var minDimension = Math.Min(width, height) - 1;
    var zoomScale = minDimension / (double)maxDimension * 1.14d;
    var upperLeftComplex = ZoomedUpperLeft(fractal, width, height, 0.5d, 0.5d, zoomScale);
    return await InitializeZoomedFractalAsync(fractal, width, height, upperLeftComplex, zoomScale, maxIterations, token);
  }

  private Task<Fractal> InitializeZoomedFractalAsync(Fractal original, int width, int height, BigComplex upperLeft, double zoomScale, int maxIterations, CancellationToken token)
  {
    var scaleReal = original.Scale.Real / (BigDecimal)zoomScale;
    var scaleImaginary = original.Scale.Imaginary / (BigDecimal)zoomScale;
    var scale = new BigComplex(scaleReal, scaleImaginary);
    return FillFractalAsync(_calculator.Type, width, height, upperLeft, scale, original.Origin, maxIterations, token);
  }

  private Task<Fractal> FillFractalAsync(string type, int width, int height, BigComplex upperLeft, BigComplex scale, BigComplex origin, int maxIterations, CancellationToken token)
  {
    return Fractal.BuildFractalAsync(type, width, height, upperLeft, scale, origin, maxIterations, _initializeProgress, token);
  }

  /// <summary>
  ///   Calculates the <see cref="BigComplex" /> upper left point from the given <see cref="Fractal" /> using the
  ///   provided image coordinates and the zoom scale used to generate those coordinates. Note that this is
  ///   not the same as the original fractal's upper left point because the user may have panned to a new
  ///   center point.
  /// </summary>
  /// <param name="original">
  ///   Fractal that is being zoomed in on
  /// </param>
  /// <param name="width">
  ///   Bitmap width to use for the image that is being calculated
  /// </param>
  /// <param name="height">
  ///   Bitmap height to use for the image that is being calculated
  /// </param>
  /// <param name="xRatio">
  /// </param>
  /// <param name="yRatio">
  /// </param>
  /// <param name="zoomScale">
  ///   The scale of the zoom
  /// </param>
  /// <returns></returns>
  private BigComplex ZoomedUpperLeft(Fractal original, int width, int height, double xRatio, double yRatio, double zoomScale)
  {
    var center = original.FindPoint((BigDecimal)xRatio, (BigDecimal)yRatio);
    var offset = (BigDecimal)Math.Max(width, height) / 2;
    var scaleX = original.Scale.Real / (BigDecimal)zoomScale;
    var scaleY = original.Scale.Imaginary / (BigDecimal)zoomScale;
    var left = center.Real - offset * scaleX;
    var upper = center.Imaginary - offset * scaleY;
    return new BigComplex(left, upper);
  }

  private void CalculateStrip(IReadOnlyCollection<ImageCoordinate> strip, int maxIterations, int totalPixels, bool useHighPrecision, CancellationToken token)
  {
    foreach (var coordinate in strip)
    {
      if (token.IsCancellationRequested)
        token.ThrowIfCancellationRequested();

      coordinate.Iterations = _calculator.Compute(coordinate.Point, maxIterations, useHighPrecision);
      coordinate.Color = _colorMapService.GetColorForIteration(coordinate.Iterations, maxIterations, _map);
    }

    ProgressHandler(strip.Count, totalPixels);
  }

  private void ProgressHandler(int pixelCount, double totalPixels)
  {
    var totalCompletedPixels = (double)Interlocked.Add(ref _totalCompletedPixels, pixelCount);
    var pct = totalCompletedPixels / totalPixels;
    _progress.Report(new FractalProgress(1d, pct, 0d));
  }
}