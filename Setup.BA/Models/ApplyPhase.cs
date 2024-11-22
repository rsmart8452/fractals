using Setup.BA.Util;
using System;
using System.Linq;
using System.Windows;
using WixToolset.BootstrapperApplicationApi;

namespace Setup.BA.Models
{
  internal class ApplyPhase
  {
    private readonly Model _model;

    public ApplyPhase(Model model)
    {
      _model = model;
    }

    public event EventHandler<EventArgs> ApplyPhaseComplete;

    /// <summary>
    ///   Fired when the engine has begun installing the bundle.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="PhaseException"></exception>
    public virtual void OnApplyBegin(object sender, ApplyBeginEventArgs e)
    {
      try
      {
        _model.State.PhaseResult = 0;
        _model.State.ErrorMessage = string.Empty;
        if (e.Cancel)
          return;

        _model.State.BaStatus = BaStatus.Applying;
      }
      catch (Exception ex)
      {
        _model.Log.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has completed installing the bundle.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="PhaseException"></exception>
    public virtual void OnApplyComplete(object sender, ApplyCompleteEventArgs e)
    {
      try
      {
        _model.State.PhaseResult = e.Status;

        // Set the state to applied or failed unless the user cancelled.
        if (_model.State.BaStatus == BaStatus.Cancelled || e.Status == ErrorHelper.CancelHResult)
        {
          _model.State.BaStatus = BaStatus.Cancelled;
          _model.Log.Write("User cancelled");
        }
        else if (ErrorHelper.HResultIsFailure(e.Status))
        {
          _model.State.BaStatus = BaStatus.Failed;
          var msg = $"Apply failed - {ErrorHelper.HResultToMessage(e.Status)}";
          if (string.IsNullOrEmpty(_model.State.ErrorMessage))
            _model.State.ErrorMessage = msg;

          _model.Log.Write(msg);

          if (e.Restart == ApplyRestart.RestartRequired && _model.UiFacade.IsUiShown)
            _model.UiFacade.ShowMessageBox(Constants.RebootMessage, MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK);
        }
        else
          _model.State.BaStatus = BaStatus.Applied;
      }
      catch (Exception ex)
      {
        _model.Log.Write(ex);
        throw new PhaseException(ex);
      }
      finally
      {
        if (_model.State.Display == Display.Full)
          ApplyPhaseComplete?.Invoke(this, EventArgs.Empty);
        else
          _model.UiFacade.ShutDown();
      }
    }

    /// <summary>
    ///   Fired when the engine has encountered an error.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="PhaseException"></exception>
    public virtual void OnError(object sender, ErrorEventArgs e)
    {
      try
      {
        if (e.ErrorCode == ErrorHelper.CancelCode)
          _model.State.BaStatus = BaStatus.Cancelled;
        else
        {
          _model.State.ErrorMessage = e.ErrorMessage;
          _model.Log.Write("Error encountered");
          _model.Log.Write(e.ErrorMessage, true);
          _model.Log.Write($"Type: {e.ErrorType}", true);
          _model.Log.Write($"Code: {e.ErrorCode}", true);
          if (!string.IsNullOrWhiteSpace(e.PackageId))
            _model.Log.Write($"Package: {e.PackageId}", true);

          var data = e.Data?.Where(d => !string.IsNullOrWhiteSpace(d)).ToArray() ?? Array.Empty<string>();
          if (data.Length > 0)
          {
            _model.Log.Write("Data:", true);
            foreach (var d in data)
              _model.Log.Write($"    {d}", true);
          }

          if (_model.UiFacade.IsUiShown)
          {
            // Show an error dialog.
            var button = MessageBoxButton.OK;
            var buttonHint = e.UIHint & 0xF;

            if (buttonHint >= 0 && buttonHint <= 4)
              button = (MessageBoxButton)buttonHint;

            var response = _model.UiFacade.ShowMessageBox(e.ErrorMessage, button, MessageBoxImage.Error, MessageBoxResult.None);

            if (buttonHint == (int)button)
            {
              // If WiX supplied a hint, return the result
              e.Result = (Result)response;
              _model.Log.Write($"User response: {response}");
            }
          }
        }
      }
      catch (Exception ex)
      {
        _model.Log.Write(ex);
        throw new PhaseException(ex);
      }
    }
  }
}