using System;

namespace FractalFlair.Models
{
  public class ImageCompleteEventArgs : EventArgs
  {
    public ImageCompleteEventArgs(ImageCoordinate[] strip)
    {
      Strip = strip;
    }

    public ImageCoordinate[] Strip { get; }
  }

}
