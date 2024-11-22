// Ignore Spelling: svc

using FractalFlair.Models;
using FractalFlair.Models.PixelCalculators;
using System.Numerics;

namespace FractalFlair.Tests;

[TestClass]
public class PixelTests
{
  /// <summary>
  ///   Tests center of image - should be infinite iterations.
  ///   Define max iterations as 255 so unit test will complete quickly
  /// </summary>
  [TestMethod]
  public void MandelbrotCenter()
  {
    IPixelCalculator svc = new MandelbrotPixel();
    var point = new Complex(0, 0);
    var actual = svc.Compute(point, 255);
    Assert.AreEqual(int.MaxValue, actual, $"Failed to calculate to infinity. Returned {actual} iterations instead.");
  }

  /// <summary>
  ///   Tests the upper left corner of image - shouldn't require any iterations
  /// </summary>
  [TestMethod]
  public void MandelbrotUpperLeft()
  {
    IPixelCalculator svc = new MandelbrotPixel();
    var point = new Complex(-2, -2);
    var actual = svc.Compute(point, 255);
    Assert.AreEqual(0, actual);
  }

  /// <summary>
  ///   Tests the lower right corner of image - shouldn't require any iterations
  /// </summary>
  [TestMethod]
  public void MandelbrotLowerRight()
  {
    IPixelCalculator svc = new MandelbrotPixel();
    var point = new Complex(2, 2);
    var actual = svc.Compute(point, 255);
    Assert.AreEqual(0, actual);
  }

  /// <summary>
  ///   Tests center of image - should be infinite iterations.
  ///   Define max iterations as 255 so unit test will complete quickly
  /// </summary>
  [TestMethod]
  public void BigMandelbrotCenter()
  {
    IPixelCalculator svc = new MandelbrotPixel();
    var point = new BigComplex(0d, 0d);
    var actual = svc.Compute(point, 255, false);
    Assert.AreEqual(int.MaxValue, actual, $"Failed to calculate to infinity. Returned {actual} iterations instead.");
  }

  /// <summary>
  ///   Tests the upper left corner of image - shouldn't require any iterations
  /// </summary>
  [TestMethod]
  public void BigMandelbrotUpperLeft()
  {
    IPixelCalculator svc = new MandelbrotPixel();
    var point = new BigComplex(-2d, -2d);
    var actual = svc.Compute(point, 255, false);
    Assert.AreEqual(0, actual);
  }

  /// <summary>
  ///   Tests the lower right corner of image - shouldn't require any iterations
  /// </summary>
  [TestMethod]
  public void BigMandelbrotLowerRight()
  {
    IPixelCalculator svc = new MandelbrotPixel();
    var point = new BigComplex(2d, 2d);
    var actual = svc.Compute(point, 255, false);
    Assert.AreEqual(0, actual);
  }

  /// <summary>
  ///   Tests center of image - should be infinite iterations.
  ///   Define max iterations as 255 so unit test will complete quickly
  /// </summary>
  [TestMethod]
  public void BigPreciseMandelbrotCenter()
  {
    IPixelCalculator svc = new MandelbrotPixel();
    var point = new BigComplex(0d, 0d);
    var actual = svc.Compute(point, 255, true);
    Assert.AreEqual(int.MaxValue, actual, $"Failed to calculate to infinity. Returned {actual} iterations instead.");
  }

  /// <summary>
  ///   Tests the upper left corner of image - shouldn't require any iterations
  /// </summary>
  [TestMethod]
  public void BigPreciseMandelbrotUpperLeft()
  {
    IPixelCalculator svc = new MandelbrotPixel();
    var point = new BigComplex(-2d, -2d);
    var actual = svc.Compute(point, 255, true);
    Assert.AreEqual(0, actual);
  }

  /// <summary>
  ///   Tests the lower right corner of image - shouldn't require any iterations
  /// </summary>
  [TestMethod]
  public void BigPreciseMandelbrotLowerRight()
  {
    IPixelCalculator svc = new MandelbrotPixel();
    var point = new BigComplex(2d, 2d);
    var actual = svc.Compute(point, 255, true);
    Assert.AreEqual(0, actual);
  }
}