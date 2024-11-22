namespace FractalFlair.Models;

public class FractalProgress
{
  public FractalProgress(double initializePercent, double calculatePercent, double colorMapPercent)
  {
    InitializePercent = initializePercent;
    CalculatePercent = calculatePercent;
    ColorMapPercent = colorMapPercent;
  }

  public double InitializePercent { get; }
  public double CalculatePercent { get; }
  public double ColorMapPercent { get; }
  public double TotalProgress => InitializePercent * 0.05 + CalculatePercent * 0.7d + ColorMapPercent * 0.25d;
}