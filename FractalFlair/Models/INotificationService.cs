using FractalFlair.Views.Controls;
using System;

namespace FractalFlair.Models;

public interface INotificationService
{
  event EventHandler<ExceptionEventArgs>? ExceptionRaised;
  event EventHandler? ResetZoomScaleView;
  event EventHandler<AnimateJuliaEventArgs>? JuliaAnimationRequested;
  event EventHandler<ZoomEventArgs>? UiZoomComplete;

  void HandleException(Exception ex);

  /// <summary>
  ///   Bubbles the PanAndZoomViewer.ZoomComplete event from the UI to app services
  /// </summary>
  /// <param name="args"></param>
  void NotifyUiZoomComplete(ZoomEventArgs args);

  void RequestZoomScaleReset();
  void RequestJuliaAnimation(BigComplex startOrigin, BigComplex endOrigin, int frames);
}