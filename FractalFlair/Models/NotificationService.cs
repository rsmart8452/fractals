using Common;
using FractalFlair.Views.Controls;
using System;

namespace FractalFlair.Models;

public class NotificationService : INotificationService
{
  private readonly WeakEventManager _eventManager = new();

  public void HandleException(Exception ex)
  {
    _eventManager.RaiseEvent(this, new ExceptionEventArgs(ex), nameof(ExceptionRaised));
  }

  public void RequestZoomScaleReset()
  {
    _eventManager.RaiseEvent(this, EventArgs.Empty, nameof(ResetZoomScaleView));
  }

  public void NotifyUiZoomComplete(ZoomEventArgs zoomArgs)
  {
    _eventManager.RaiseEvent(this, zoomArgs, nameof(UiZoomComplete));
  }

  public void RequestJuliaAnimation(BigComplex startOrigin, BigComplex endOrigin, int frames)
  {
    var args = new AnimateJuliaEventArgs(startOrigin, endOrigin, frames);
    _eventManager.RaiseEvent(this, args, nameof(ResetZoomScaleView));
  }

  public event EventHandler<AnimateJuliaEventArgs>? JuliaAnimationRequested
  {
    add => _eventManager.AddEventHandler(value);
    remove => _eventManager.RemoveEventHandler(value);
  }

  public event EventHandler<ExceptionEventArgs>? ExceptionRaised
  {
    add => _eventManager.AddEventHandler(value);
    remove => _eventManager.RemoveEventHandler(value);
  }

  public event EventHandler? ResetZoomScaleView
  {
    add => _eventManager.AddEventHandler(value);
    remove => _eventManager.RemoveEventHandler(value);
  }

  public event EventHandler<ZoomEventArgs>? UiZoomComplete
  {
    add => _eventManager.AddEventHandler(value);
    remove => _eventManager.RemoveEventHandler(value);
  }
}