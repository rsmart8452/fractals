using FractalFlair.Models;
using FractalFlair.Models.PixelCalculators;
using System.Diagnostics;
using System.Text;

namespace FractalFlair.Tests;

[TestClass]
public class MandelbrotTests
{
  private const int _imageSize = 21;
  private const int _maxIterations = 20;

  [TestMethod]
  public async Task Calculate()
  {
    var calculator = new FractalCalculator(new MandelbrotPixel(), ColorMaps.Default(), new Progress<FractalProgress>());
    var result = await calculator.CalculateAsync(_imageSize, _imageSize, _maxIterations, CancellationToken.None);
    var upperLeftResult = result.ImageCoordinates[0][0];
    var centerResult = result.ImageCoordinates[_imageSize / 2 - 1][_imageSize / 2 - 1];
    Assert.AreEqual(0, upperLeftResult.Iterations, $"Number of calculated iterations should be zero, instead {upperLeftResult.Iterations} were calculated.");
    Assert.AreEqual(int.MaxValue, centerResult.Iterations, $"Number of iteration in the fractal center should have been infinite, instead {centerResult.Iterations} were calculated");

    PrintFractal(result);
  }

  //[TestMethod]
  //public void HighPrecisionCalculate()
  //{
  //  const double zoomCenterX = 50d;
  //  const double zoomCenterY = 90d;
  //  const double zoomScale = 10d;

  //  var calculator = new FractalCalculator(new MandelbrotPixel(), ColorMaps.Default(), new Progress<FractalProgress>());
  //  var originalFractal = calculator.Calculate(_imageSize, _imageSize, _maxIterations, CancellationToken.None);
  //  var upperLeftResult = originalFractal.ImageCoordinates[0][0];
  //  var centerResult = originalFractal.ImageCoordinates[_imageSize / 2 - 1][_imageSize / 2 - 1];
  //  Assert.AreEqual(0, upperLeftResult.Iterations, $"Number of calculated iterations should be zero, instead {upperLeftResult.Iterations} were calculated.");
  //  Assert.AreEqual(int.MaxValue, centerResult.Iterations, $"Number of iteration in the fractal center should have been infinite, instead {centerResult.Iterations} were calculated");

  //  var result = calculator.Calculate(originalFractal, 3, 3, zoomCenterX, zoomCenterY, zoomScale, _maxIterations * 5, true, CancellationToken.None);
  //  PrintFractal(result);
  //}

  [TestMethod]
  public async Task FindCenter()
  {
    // Test requires a large enough image such that the final zoomed fractal can be found within its coordinates
    // These values insure that the test conditions are met.
    const double zoomFactor = 10d;
    const int movedX = 5;
    const int movedY = 9;

    var calculator = new FractalCalculator(new MandelbrotPixel(), ColorMaps.Default(), new Progress<FractalProgress>());
    var originalFractal = await calculator.CalculateAsync(_imageSize, _imageSize, _maxIterations, CancellationToken.None);
    var center = originalFractal.ImageCoordinates[10][10].Point;
    Debug.WriteLine($"20X20 center:     X = {(double)center.Real:0.000}   Y = {(double)center.Imaginary:0.000}");
    Assert.IsTrue(Math.Abs((double)center.Real + 0.5d) < 0.00001d, "Wrong Real value was calculated. X coordinate of center point didn't calculate correctly.");
    Assert.IsTrue(Math.Abs((double)center.Imaginary) < 0.00001d, "Wrong Imaginary value was calculated. Y coordinate of center point didn't calculate correctly.");

    var movedCenter = originalFractal.ImageCoordinates[movedX][movedY];
    Debug.WriteLine(string.Empty);
    Debug.WriteLine("Moved to:");
    Debug.WriteLine($"Image coords:     X = {movedCenter.Pixel.X}   Y = {movedCenter.Pixel.Y}");
    Debug.WriteLine($"Coords:           X = {(double)movedCenter.Point.Real:0.000}   Y = {(double)movedCenter.Point.Imaginary:0.000}");
    Debug.WriteLine(string.Empty);

    var zoomSize = (int)Math.Round((_imageSize - 1d) * zoomFactor) + 1;
    var zoomScale = (zoomSize - 1d) / (_imageSize - 1d); // Should be exactly 10
    Debug.WriteLine(string.Empty);
    Debug.WriteLine($"Zoom scale:       {zoomScale:0.000}");
    Debug.WriteLine(string.Empty);

    var zoomedFractal = await calculator.CalculateAsync(zoomSize, zoomSize, _maxIterations, CancellationToken.None);
    var zoomedImageSize = (_imageSize - 1) * zoomFactor + 1;
    var centerIdx = (int)Math.Round((zoomedImageSize - 1) / 2d);
    var centerZoomed = zoomedFractal.ImageCoordinates[centerIdx][centerIdx].Point;
    Debug.WriteLine($"X = {(double)centerZoomed.Real:0.000}   Y = {(double)centerZoomed.Imaginary:0.000}");
    Assert.IsTrue(Math.Abs((double)centerZoomed.Real + 0.5d) < 0.00001d, "Wrong Real value was calculated. X coordinate of center point didn't calculate correctly.");
    Assert.IsTrue(Math.Abs((double)centerZoomed.Imaginary) < 0.00001d, "Wrong Imaginary value was calculated. Y coordinate of center point didn't calculate correctly.");

    var zoomedMoveX = (int)Math.Round(movedX * zoomFactor);
    var zoomedMoveY = (int)Math.Round(movedY * zoomFactor);
    var movedCenterZoom = zoomedFractal.ImageCoordinates[zoomedMoveX][zoomedMoveY];
    Debug.WriteLine("Zoomed and moved to:");
    Debug.WriteLine($"Image coords:     X = {movedCenterZoom.Pixel.X}   Y = {movedCenterZoom.Pixel.Y}");
    Debug.WriteLine($"Coords:           X = {(double)movedCenterZoom.Point.Real:0.000}   Y = {(double)movedCenterZoom.Point.Imaginary:0.000}");
    Debug.WriteLine(string.Empty);

    Debug.WriteLine($"Scale factor:     {zoomScale:0.000000}");
    var centerRatioX = movedCenter.Pixel.X / (double)originalFractal.ImageCoordinates.Length;
    var centerRatioY = movedCenter.Pixel.Y / (double)originalFractal.ImageCoordinates[0].Length;

    var result = await calculator.CalculateAsync(originalFractal, _imageSize, _imageSize, centerRatioX, centerRatioY, zoomScale, _maxIterations * 5, false, CancellationToken.None);

    var halfSize = (_imageSize - 1) / 2;
    var upperLeftExpected = zoomedFractal.ImageCoordinates[movedCenterZoom.Pixel.X - halfSize][movedCenterZoom.Pixel.Y - halfSize];
    var upperLeftActual = result.ImageCoordinates[0][0];
    Debug.WriteLine($"Upper left expected: X = {(double)upperLeftExpected.Point.Real:0.000}   Y = {(double)upperLeftExpected.Point.Imaginary:0.000}");
    Debug.WriteLine($"Upper left actual:   X = {(double)upperLeftActual.Point.Real:0.000}   Y = {(double)upperLeftActual.Point.Imaginary:0.000}");
    Debug.WriteLine(string.Empty);
    PrintFractal(result);
    //Assert.IsTrue(Math.Abs((double)(upperLeftActual.Point.Real - upperLeftExpected.Point.Real)) < 0.0001d, $"Wrong Real value was found. Should be close to {(double)upperLeftExpected.Point.Real:0.000}");
    //Assert.IsTrue(Math.Abs((double)(upperLeftActual.Point.Imaginary - upperLeftExpected.Point.Imaginary)) < 0.0001d, $"Wrong Imaginary value was found. Should be close to {(double)upperLeftExpected.Point.Imaginary:0.000}");
  }

  private void PrintFractal(Fractal fractal)
  {
    var width = fractal.ImageCoordinates.Length;
    var height = fractal.ImageCoordinates[0].Length;
    var sb = new StringBuilder();
    for (var heightIdx = 0; heightIdx < height; heightIdx++)
    {
      for (var widthIdx = 0; widthIdx < width; widthIdx++)
      {
        var coordinate = fractal.ImageCoordinates[widthIdx][heightIdx];
        sb.Append(
          coordinate.Iterations == int.MaxValue
            ? "██"
            : coordinate.Iterations.ToString("00"));

        if (widthIdx < width - 1)
          sb.Append(' ');
      }

      sb.AppendLine();
    }

    Debug.Write(sb.ToString());
  }
}