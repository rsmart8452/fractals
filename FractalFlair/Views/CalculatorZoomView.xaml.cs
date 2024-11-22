// Ignore Spelling: Vm

using FractalFlair.Models;
using FractalFlair.Views.Controls;
using System;

namespace FractalFlair.Views;

public partial class CalculatorZoomView
{
  private INotificationService? _resetNotifier;

  public INotificationService? ZoomNotifier;

  public CalculatorZoomView()
  {
    InitializeComponent();
    Viewer.ZoomComplete += Viewer_ZoomComplete;
  }

  public INotificationService? ResetNotifier
  {
    set
    {
      if (_resetNotifier == value)
        return;

      if (_resetNotifier != null)
        _resetNotifier.ResetZoomScaleView -= ResetNotifier_ResetZoomScaleView;

      _resetNotifier = value;
      if (_resetNotifier != null)
        _resetNotifier.ResetZoomScaleView += ResetNotifier_ResetZoomScaleView;
    }
  }

  private void ResetNotifier_ResetZoomScaleView(object? sender, EventArgs e)
  {
    Viewer.ForceReset();
  }

  private void Viewer_ZoomComplete(object sender, ZoomEventArgs zoomArgs)
  {
    ZoomNotifier?.NotifyUiZoomComplete(zoomArgs);
  }
}