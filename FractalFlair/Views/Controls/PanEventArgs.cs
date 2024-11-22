using System;
using System.Windows;
using System.Windows.Input;

namespace FractalFlair.Views.Controls;

public class PanEventArgs : EventArgs
{
  public PanEventArgs(Point location, bool cancel)
  {
    Cancel = cancel;
    Location = location;
  }

  public bool Cancel { get; }
  public Point? Location { get; }
  public MouseWheelEventArgs? MouseEvents { get; set; }
  public double CurrentZoomLevel { get; set; }
  public Point? NoZoomPoint { get; set; }
}