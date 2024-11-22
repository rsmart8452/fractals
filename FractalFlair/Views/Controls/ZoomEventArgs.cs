namespace FractalFlair.Views.Controls;

public class ZoomEventArgs
{
  public ZoomEventArgs(double zoomFactor)
  {
    ZoomFactor = zoomFactor;
  }

  public double ZoomFactor { get; }
}