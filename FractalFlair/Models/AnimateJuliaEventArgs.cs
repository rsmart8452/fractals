using System;

namespace FractalFlair.Models;

public class AnimateJuliaEventArgs : EventArgs
{
  public AnimateJuliaEventArgs(BigComplex startOrigin, BigComplex endOrigin, int frames)
  {
    StartOrigin = startOrigin;
    EndOrigin = endOrigin;
    Frames = frames;
  }

  public BigComplex StartOrigin { get; }
  public BigComplex EndOrigin { get; }
  public int Frames { get; }
}