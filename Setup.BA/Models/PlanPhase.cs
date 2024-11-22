using Setup.BA.Util;
using System;
using WixToolset.BootstrapperApplicationApi;

namespace Setup.BA.Models
{
  internal class PlanPhase
  {
    private readonly Model _model;

    public PlanPhase(Model model)
    {
      _model = model;
    }

    public event EventHandler<EventArgs> PlanPhaseFailed;

    /// <summary>
    ///   Fired when the engine has begun planning the installation.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="PhaseException"></exception>
    public virtual void OnPlanBegin(object sender, PlanBeginEventArgs e)
    {
      try
      {
        _model.State.PhaseResult = 0;
        _model.State.ErrorMessage = string.Empty;

        if (e.Cancel)
          return;

        _model.State.BaStatus = BaStatus.Planning;
      }
      catch (Exception ex)
      {
        _model.Log.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has completed planning the installation.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="PhaseException"></exception>
    public virtual void OnPlanComplete(object sender, PlanCompleteEventArgs e)
    {
      try
      {
        _model.State.PhaseResult = e.Status;

        if (_model.State.BaStatus == BaStatus.Cancelled || e.Status == ErrorHelper.CancelHResult)
        {
          _model.State.BaStatus = BaStatus.Cancelled;
          _model.Log.Write("User cancelled");
        }
        else if (ErrorHelper.HResultIsFailure(e.Status))
        {
          _model.State.BaStatus = BaStatus.Failed;
          var msg = $"Plan failed - {ErrorHelper.HResultToMessage(e.Status)}";
          if (string.IsNullOrEmpty(_model.State.ErrorMessage))
            _model.State.ErrorMessage = msg;

          _model.Log.Write(msg);

          if (_model.UiFacade.IsUiShown)
            PlanPhaseFailed?.Invoke(this, EventArgs.Empty);
          else
            _model.UiFacade.ShutDown();
        }
        else
        {
          _model.Log.Write("Plan succeeded, starting apply phase");
          _model.Engine.Apply(_model.UiFacade.ShellWindowHandle);
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