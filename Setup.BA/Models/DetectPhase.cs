﻿using Setup.BA.Util;
using System;
using WixToolset.BootstrapperApplicationApi;

namespace Setup.BA.Models
{
  internal class DetectPhase
  {
    private readonly Model _model;
    private DetectionState _bundleDetectedState;

    public DetectPhase(Model model)
    {
      _model = model;
      _bundleDetectedState = DetectionState.Unknown;
    }

    public event EventHandler<DetectPhaseCompleteEventArgs> DetectPhaseComplete;

    /// <summary>
    ///   Fired when the overall detection phase has begun.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="PhaseException"></exception>
    public virtual void OnDetectBegin(object sender, DetectBeginEventArgs e)
    {
      try
      {
        _model.State.BaStatus = BaStatus.Detecting;
        if (e.RegistrationType == RegistrationType.Full)
          _bundleDetectedState = DetectionState.Present;
        else
          _bundleDetectedState = DetectionState.Absent;

        _model.Log.Write($"{nameof(_bundleDetectedState)} set to {_bundleDetectedState}");
      }
      catch (Exception ex)
      {
        _model.Log.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when a related bundle has been detected for a bundle.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <remarks>
    ///   Helpful when the detected bundle has the same upgrade code that this one does.
    /// </remarks>
    public virtual void OnDetectRelatedBundle(object sender, DetectRelatedBundleEventArgs e)
    {
      try
      {
        // If the detected bundle's upgrade code matches this bundle, but product code and version are different...
        if (e.RelationType == RelationType.Upgrade)
        {
          _bundleDetectedState = DetectionState.Present;
          if (string.IsNullOrWhiteSpace(_model.State.RelatedBundleVersion))
            _model.State.RelatedBundleVersion = e.Version;

          _model.Log.Write($"Detected version = {e.Version} / Bundle version = {_model.State.BundleVersion}");

          if (_model.Engine.CompareVersions(_model.State.BundleVersion, e.Version) > 0)
          {
            if (_model.State.RelatedBundleStatus <= BundleStatus.Current)
              _model.State.RelatedBundleStatus = BundleStatus.OlderInstalled;
          }
          else if (_model.Engine.CompareVersions(_model.State.BundleVersion, e.Version) == 0)
          {
            if (_model.State.RelatedBundleStatus == BundleStatus.NotInstalled)
              _model.State.RelatedBundleStatus = BundleStatus.Current;
          }
          else
            _model.State.RelatedBundleStatus = BundleStatus.NewerInstalled;

          _model.Log.Write($"{nameof(_model.State.RelatedBundleStatus)} set to {_model.State.RelatedBundleStatus}");
        }

        if (!_model.State.Bundle.Packages.ContainsKey(e.ProductCode))
        {
          var package = _model.State.Bundle.AddRelatedBundleAsPackage(e.ProductCode, e.RelationType, e.PerMachine, e.Version);
          _model.State.RelatedBundleId = package.Id;
          if (_model.Engine.ContainsVariable(Constants.BundleNameVariable))
          {
            var name = _model.Engine.GetVariableString(Constants.BundleNameVariable);
            _model.State.RelatedBundleName = $"{name} v{_model.State.RelatedBundleVersion}";
          }
          else
            _model.State.RelatedBundleName = $"v{_model.State.RelatedBundleVersion}";
        }
      }
      catch (Exception ex)
      {
        _model.Log.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the detection phase has completed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="PhaseException"></exception>
    public virtual void OnDetectComplete(object sender, DetectCompleteEventArgs e)
    {
      var followupAction = LaunchAction.Unknown;
      try
      {
        _model.State.PhaseResult = e.Status;

        if (ErrorHelper.HResultIsFailure(e.Status))
        {
          var msg = $"Detect failed - {ErrorHelper.HResultToMessage(e.Status)}";
          _model.Log.Write(msg);

          if (string.IsNullOrEmpty(_model.State.ErrorMessage))
            _model.State.ErrorMessage = msg;

          if (!_model.UiFacade.IsUiShown)
            _model.UiFacade.ShutDown();

          return;
        }

        if (_model.State.RelatedBundleStatus == BundleStatus.Unknown)
        {
          if (_bundleDetectedState == DetectionState.Present)
          {
            _model.State.RelatedBundleStatus = BundleStatus.Current;
            if (string.IsNullOrWhiteSpace(_model.State.RelatedBundleVersion))
              _model.State.RelatedBundleVersion = _model.State.BundleVersion;
          }
          else
            _model.State.RelatedBundleStatus = BundleStatus.NotInstalled;
        }

        _model.State.BaStatus = BaStatus.Waiting;

        if (_model.CommandInfo.Action == LaunchAction.Uninstall && _model.CommandInfo.Resume == ResumeType.Arp)
        {
          _model.Log.Write("Starting plan for automatic uninstall");
          followupAction = _model.CommandInfo.Action;
        }
        else if (_model.State.Display != Display.Full)
        {
          _model.Log.Write("Starting plan for silent mode.");
          followupAction = _model.CommandInfo.Action;
        }
      }
      catch (Exception ex)
      {
        _model.Log.Write(ex);
        throw new PhaseException(ex);
      }
      finally
      {
        // Can't start plan until UI is notified
        NotifyDetectComplete(followupAction);
      }

      try
      {
        if (followupAction != LaunchAction.Unknown)
          _model.PlanAndApply(followupAction);
      }
      catch (Exception ex)
      {
        _model.Log.Write(ex);
        throw new PhaseException(ex);
      }
    }

    private void NotifyDetectComplete(LaunchAction followupAction)
    {
      if (_model.UiFacade.IsUiShown)
        DetectPhaseComplete?.Invoke(this, new DetectPhaseCompleteEventArgs(followupAction));
    }
  }
}