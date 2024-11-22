using Common;
using FractalFlair.Models;
using JetBrains.Annotations;
using System;
using System.Windows.Threading;

namespace FractalFlair.Views.Controls;

/// <summary>
///   Bubbles <see cref="INotificationService" /> events on the UI thread
/// </summary>
[UsedImplicitly]
public class UiNotificationService : INotificationService, IDisposable
{
  private readonly INotificationService _notificationService;
  private readonly Dispatcher _dispatcher;
  private readonly WeakEventManager _eventManager = new();

  private bool _disposed;

  public UiNotificationService(INotificationService notificationService, Dispatcher dispatcher)
  {
    // UiZoomComplete is not bubbled to the UI, so it doesn't send notifications.
    _notificationService = notificationService;
    _notificationService.ExceptionRaised += NotificationService_ExceptionRaised;
    _notificationService.ResetZoomScaleView += NotificationService_ResetZoomScaleView;
    _notificationService.JuliaAnimationRequested += NotificationService_JuliaAnimationRequested;

    _dispatcher = dispatcher;
    _dispatcher.UnhandledException += Dispatcher_UnhandledException;
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

  public void HandleException(Exception ex)
  {
    _eventManager.RaiseEvent(this, new ExceptionEventArgs(ex), nameof(ExceptionRaised));
  }

  public void NotifyUiZoomComplete(ZoomEventArgs zoomArgs)
  {
    throw new NotImplementedException("Services should never notify the UI that the zoom level has changed");
  }

  public void RequestZoomScaleReset()
  {
    _eventManager.RaiseEvent(this, EventArgs.Empty, nameof(ResetZoomScaleView));
  }

  public void RequestJuliaAnimation(BigComplex startOrigin, BigComplex endOrigin, int frames)
  {
    var args = new AnimateJuliaEventArgs(startOrigin, endOrigin, frames);
    _eventManager.RaiseEvent(this, args, nameof(JuliaAnimationRequested));
  }

  public void Dispose()
  {
    if (_disposed)
      return;

    Dispose(true);
    GC.SuppressFinalize(this);
  }

  protected virtual void Dispose([UsedImplicitly] bool disposeManagedResources)
  {
    if (_disposed)
      return;

    // Dispose unmanaged resources
    _notificationService.ExceptionRaised -= NotificationService_ExceptionRaised;
    _notificationService.ResetZoomScaleView -= NotificationService_ResetZoomScaleView;
    _notificationService.JuliaAnimationRequested -= NotificationService_JuliaAnimationRequested;
    _dispatcher.UnhandledException -= Dispatcher_UnhandledException;

    _disposed = true;
  }

  private void Dispatcher_UnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
  {
    _dispatcher.Invoke(() => HandleException(e.Exception));
    e.Handled = true;
  }

  private void NotificationService_JuliaAnimationRequested(object? sender, AnimateJuliaEventArgs e)
  {
    _dispatcher.BeginInvoke(() => RequestJuliaAnimation(e.StartOrigin, e.EndOrigin, e.Frames));
  }

  private void NotificationService_ResetZoomScaleView(object? sender, EventArgs e)
  {
    _dispatcher.BeginInvoke(RequestZoomScaleReset);
  }

  private void NotificationService_ExceptionRaised(object? sender, ExceptionEventArgs e)
  {
    _dispatcher.Invoke(() => HandleException(e.Exception));
  }

  ~UiNotificationService()
  {
    Dispose(false);
  }
}