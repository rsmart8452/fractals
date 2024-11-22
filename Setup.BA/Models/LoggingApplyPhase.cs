using Setup.BA.Util;
using System;
using System.Linq;
using WixToolset.BootstrapperApplicationApi;

namespace Setup.BA.Models
{
  internal class LoggingApplyPhase : ApplyPhase
  {
    private readonly Log _logger;

    public LoggingApplyPhase(Model model)
      : base(model)
    {
      _logger = model.Log;
    }

    /// <summary>
    ///   Fired when the engine has begun installing the bundle.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="PhaseException"></exception>
    public override void OnApplyBegin(object sender, ApplyBeginEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnApplyBegin)} -------v");
        _logger.Write($"{nameof(e.PhaseCount)} = {e.PhaseCount}", true);

        base.OnApplyBegin(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnApplyBegin)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has completed installing the bundle.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="PhaseException"></exception>
    public override void OnApplyComplete(object sender, ApplyCompleteEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnApplyComplete)} -------v");
        _logger.Write($"{nameof(e.Status)} = {ErrorHelper.HResultToMessage(e.Status)}", true);
        _logger.Write($"{nameof(e.Restart)} = {e.Restart}", true);
        _logger.Write($"{nameof(e.Recommendation)} = {e.Recommendation}", true);

        base.OnApplyComplete(sender, e);

        _logger.Write($"{nameof(e.Action)} = {e.Action}", true);
        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnApplyComplete)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the plan determined that nothing should happen to prevent downgrading.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnApplyDowngrade(object sender, ApplyDowngradeEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnApplyDowngrade)} -------v");
        _logger.Write($"{nameof(e.Status)} = {ErrorHelper.HResultToMessage(e.Status)}", true);
        _logger.Write($"{nameof(e.Recommendation)} = {e.Recommendation}", true);

        // base.OnApplyDowngrade(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnApplyDowngrade)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has begun installing packages.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnExecuteBegin(object sender, ExecuteBeginEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnExecuteBegin)} -------v");
        _logger.Write($"{nameof(e.PackageCount)} = {e.PackageCount}", true);

        // base.OnExecuteBegin(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnExecuteBegin)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has completed installing packages.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnExecuteComplete(object sender, ExecuteCompleteEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnExecuteComplete)} -------v");
        _logger.Write($"{nameof(e.Status)} = {ErrorHelper.HResultToMessage(e.Status)}", true);

        // base.OnExecuteComplete(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnExecuteComplete)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired by the engine while executing a package.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnExecuteProgress(object sender, ExecuteProgressEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnExecuteProgress)} -------v");
        _logger.Write($"{nameof(e.PackageId)} = {e.PackageId}", true);
        _logger.Write($"{nameof(e.OverallPercentage)} = {e.OverallPercentage}", true);
        _logger.Write($"{nameof(e.ProgressPercentage)} = {e.ProgressPercentage}", true);

        // base.OnExecuteProgress(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnExecuteProgress)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine executes one or more patches targeting a product.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnExecutePatchTarget(object sender, ExecutePatchTargetEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnExecutePatchTarget)} -------v");
        _logger.Write($"{nameof(e.PackageId)} = {e.PackageId}", true);
        _logger.Write($"{nameof(e.TargetProductCode)} = {e.TargetProductCode}", true);

        // base.OnExecutePatchTarget(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnExecutePatchTarget)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine is about to begin an MSI transaction.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnBeginMsiTransactionBegin(object sender, BeginMsiTransactionBeginEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnBeginMsiTransactionBegin)} -------v");
        _logger.Write($"{nameof(e.TransactionId)} = {e.TransactionId}", true);

        // base.OnBeginMsiTransactionBegin(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnBeginMsiTransactionBegin)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has completed beginning an MSI transaction.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnBeginMsiTransactionComplete(object sender, BeginMsiTransactionCompleteEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnBeginMsiTransactionComplete)} -------v");
        _logger.Write($"{nameof(e.Status)} = {ErrorHelper.HResultToMessage(e.Status)}", true);
        _logger.Write($"{nameof(e.TransactionId)} = {e.TransactionId}", true);

        // base.OnBeginMsiTransactionComplete(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnBeginMsiTransactionComplete)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine is about to commit an MSI transaction.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCommitMsiTransactionBegin(object sender, CommitMsiTransactionBeginEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCommitMsiTransactionBegin)} -------v");
        _logger.Write($"{nameof(e.TransactionId)} = {e.TransactionId}", true);

        // base.OnCommitMsiTransactionBegin(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCommitMsiTransactionBegin)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has completed commiting an MSI transaction.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCommitMsiTransactionComplete(object sender, CommitMsiTransactionCompleteEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCommitMsiTransactionComplete)} -------v");
        _logger.Write($"{nameof(e.Status)} = {ErrorHelper.HResultToMessage(e.Status)}", true);
        _logger.Write($"{nameof(e.TransactionId)} = {e.TransactionId}", true);
        _logger.Write($"{nameof(e.Recommendation)} = {e.Recommendation}", true);

        // base.OnCommitMsiTransactionComplete(sender, e);

        _logger.Write($"{nameof(e.Action)} = {e.Action}", true);
        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCommitMsiTransactionComplete)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine is about to roll back an MSI transaction.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnRollbackMsiTransactionBegin(object sender, RollbackMsiTransactionBeginEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnRollbackMsiTransactionBegin)} -------v");
        _logger.Write($"{nameof(e.TransactionId)} = {e.TransactionId}", true);

        // base.OnRollbackMsiTransactionBegin(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnRollbackMsiTransactionBegin)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has completed rolling back an MSI transaction.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnRollbackMsiTransactionComplete(object sender, RollbackMsiTransactionCompleteEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnRollbackMsiTransactionComplete)} -------v");
        _logger.Write($"{nameof(e.Status)} = {ErrorHelper.HResultToMessage(e.Status)}", true);
        _logger.Write($"{nameof(e.TransactionId)} = {e.TransactionId}", true);
        _logger.Write($"{nameof(e.Restart)} = {e.Restart}", true);
        _logger.Write($"{nameof(e.Recommendation)} = {e.Recommendation}", true);

        // base.OnRollbackMsiTransactionComplete(sender, e);

        _logger.Write($"{nameof(e.Action)} = {e.Action}", true);
        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnRollbackMsiTransactionComplete)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has begun acquiring the payload or container.
    ///   The BA can change the source using
    ///   <see cref="M:WixToolset.Mba.Core.IEngine.SetLocalSource(System.String,System.String,System.String)" />
    ///   or
    ///   <see
    ///     cref="M:WixToolset.Mba.Core.IEngine.SetDownloadSource(System.String,System.String,System.String,System.String,System.String)" />
    ///   .
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCacheAcquireBegin(object sender, CacheAcquireBeginEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheAcquireBegin)} -------v");
        _logger.Write($"{nameof(e.PayloadId)} = {e.PayloadId}", true);
        _logger.Write($"{nameof(e.PayloadContainerId)} = {e.PayloadContainerId}", true);
        _logger.Write($"{nameof(e.PackageOrContainerId)} = {e.PackageOrContainerId}", true);
        _logger.Write($"{nameof(e.DownloadUrl)} = {e.DownloadUrl}", true);
        _logger.Write($"{nameof(e.Source)} = {e.Source}", true);
        _logger.Write($"{nameof(e.Recommendation)} = {e.Recommendation}", true);

        // base.OnCacheAcquireBegin(sender, e);

        _logger.Write($"{nameof(e.Action)} = {e.Action}", true);
        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheAcquireBegin)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has completed the acquisition of the payload or container.
    ///   The BA can change the source using
    ///   <see cref="M:WixToolset.Mba.Core.IEngine.SetLocalSource(System.String,System.String,System.String)" />
    ///   or
    ///   <see
    ///     cref="M:WixToolset.Mba.Core.IEngine.SetDownloadSource(System.String,System.String,System.String,System.String,System.String)" />
    ///   .
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCacheAcquireComplete(object sender, CacheAcquireCompleteEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheAcquireComplete)} -------v");
        _logger.Write($"{nameof(e.Status)} = {ErrorHelper.HResultToMessage(e.Status)}", true);
        _logger.Write($"{nameof(e.PayloadId)} = {e.PayloadId}", true);
        _logger.Write($"{nameof(e.PackageOrContainerId)} = {e.PackageOrContainerId}", true);
        _logger.Write($"{nameof(e.Recommendation)} = {e.Recommendation}", true);

        // base.OnCacheAcquireComplete(sender, e);

        _logger.Write($"{nameof(e.Action)} = {e.Action}", true);
        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheAcquireComplete)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired by the engine to allow the BA to override the acquisition action.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCacheAcquireResolving(object sender, CacheAcquireResolvingEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheAcquireResolving)} -------v");
        _logger.Write($"{nameof(e.PayloadId)} = {e.PayloadId}", true);
        _logger.Write($"{nameof(e.PayloadContainerId)} = {e.PayloadContainerId}", true);
        _logger.Write($"{nameof(e.PackageOrContainerId)} = {e.PackageOrContainerId}", true);
        _logger.Write($"{nameof(e.DownloadUrl)} = {e.DownloadUrl}", true);
        _logger.Write($"{nameof(e.FoundLocal)} = {e.FoundLocal}", true);
        _logger.Write($"{nameof(e.RecommendedSearchPath)} = {e.RecommendedSearchPath}", true);
        _logger.Write($"{nameof(e.SearchPaths)} (count = {e.SearchPaths?.Length ?? 0})", true);
        if (e.SearchPaths != null)
        {
          for (var i = 0; i < e.SearchPaths.Length; i++)
            _logger.Write($"    {i} = {e.SearchPaths[i]}", true);
        }

        _logger.Write($"{nameof(e.ChosenSearchPath)} = {e.ChosenSearchPath}", true);
        _logger.Write($"{nameof(e.Action)} = {e.Action}", true);
        _logger.Write($"{nameof(e.Recommendation)} = {e.Recommendation}", true);

        // base.OnCacheAcquireResolving(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheAcquireResolving)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has progress acquiring the payload or container.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCacheAcquireProgress(object sender, CacheAcquireProgressEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheAcquireProgress)} -------v");
        LogCacheProgress(e);

        // base.OnCacheAcquireProgress(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheAcquireProgress)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has begun caching the installation sources.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCacheBegin(object sender, CacheBeginEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheBegin)} -------v");

        // base.OnCacheBegin(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheBegin)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired after the engine has cached the installation sources.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCacheComplete(object sender, CacheCompleteEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheComplete)} -------v");
        _logger.Write($"{nameof(e.Status)} = {ErrorHelper.HResultToMessage(e.Status)}", true);

        // base.OnCacheComplete(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheComplete)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has begun caching a specific package.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCachePackageBegin(object sender, CachePackageBeginEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCachePackageBegin)} -------v");
        _logger.Write($"{nameof(e.PackageId)} = {e.PackageId}", true);
        _logger.Write($"{nameof(e.Vital)} = {e.Vital}", true);
        _logger.Write($"{nameof(e.PackageCacheSize)} = {e.PackageCacheSize}", true);
        _logger.Write($"{nameof(e.CachePayloads)} = {e.CachePayloads}", true);

        // base.OnCachePackageBegin(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCachePackageBegin)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has completed caching a specific package.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCachePackageComplete(object sender, CachePackageCompleteEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCachePackageComplete)} -------v");
        _logger.Write($"{nameof(e.Status)} = {ErrorHelper.HResultToMessage(e.Status)}", true);
        _logger.Write($"{nameof(e.PackageId)} = {e.PackageId}", true);
        _logger.Write($"{nameof(e.Recommendation)} = {e.Recommendation}", true);

        // base.OnCachePackageComplete(sender, e);

        _logger.Write($"{nameof(e.Action)} = {e.Action}", true);
        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCachePackageComplete)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine failed validating a package in the package cache that is non-vital to execution.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCachePackageNonVitalValidationFailure(object sender, CachePackageNonVitalValidationFailureEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCachePackageNonVitalValidationFailure)} -------v");
        _logger.Write($"{nameof(e.Status)} = {ErrorHelper.HResultToMessage(e.Status)}", true);
        _logger.Write($"{nameof(e.PackageId)} = {e.PackageId}", true);
        _logger.Write($"{nameof(e.Recommendation)} = {e.Recommendation}", true);

        // base.OnCachePackageNonVitalValidationFailure(sender, e);

        _logger.Write($"{nameof(e.Action)} = {e.Action}", true);
        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCachePackageNonVitalValidationFailure)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine begins the verification of the payload or container that was already in the package cache.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCacheContainerOrPayloadVerifyBegin(object sender, CacheContainerOrPayloadVerifyBeginEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheContainerOrPayloadVerifyBegin)} -------v");
        _logger.Write($"{nameof(e.PayloadId)} = {e.PayloadId}", true);
        _logger.Write($"{nameof(e.PackageOrContainerId)} = {e.PackageOrContainerId}", true);

        // base.OnCacheContainerOrPayloadVerifyBegin(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheContainerOrPayloadVerifyBegin)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has completed the verification of the payload or container that was already in the package
    ///   cache.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCacheContainerOrPayloadVerifyComplete(object sender, CacheContainerOrPayloadVerifyCompleteEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheContainerOrPayloadVerifyComplete)} -------v");
        _logger.Write($"{nameof(e.Status)} = {ErrorHelper.HResultToMessage(e.Status)}", true);
        _logger.Write($"{nameof(e.PayloadId)} = {e.PayloadId}", true);
        _logger.Write($"{nameof(e.PackageOrContainerId)} = {e.PackageOrContainerId}", true);

        // base.OnCacheContainerOrPayloadVerifyComplete(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheContainerOrPayloadVerifyComplete)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine begins the extraction of the payload from the container.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCachePayloadExtractBegin(object sender, CachePayloadExtractBeginEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCachePayloadExtractBegin)} -------v");
        _logger.Write($"{nameof(e.ContainerId)} = {e.ContainerId}", true);
        _logger.Write($"{nameof(e.PayloadId)} = {e.PayloadId}", true);

        // base.OnCachePayloadExtractBegin(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCachePayloadExtractBegin)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has completed the extraction of the payload from the container.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCachePayloadExtractComplete(object sender, CachePayloadExtractCompleteEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCachePayloadExtractComplete)} -------v");
        _logger.Write($"{nameof(e.Status)} = {ErrorHelper.HResultToMessage(e.Status)}", true);
        _logger.Write($"{nameof(e.ContainerId)} = {e.ContainerId}", true);
        _logger.Write($"{nameof(e.PayloadId)} = {e.PayloadId}", true);

        // base.OnCachePayloadExtractComplete(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCachePayloadExtractComplete)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has progress extracting the payload from the container.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCachePayloadExtractProgress(object sender, CachePayloadExtractProgressEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCachePayloadExtractProgress)} -------v");
        LogCacheProgress(e);

        // base.OnCachePayloadExtractProgress(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCachePayloadExtractProgress)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine begins the verification of the acquired payload or container.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCacheVerifyBegin(object sender, CacheVerifyBeginEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheVerifyBegin)} -------v");
        _logger.Write($"{nameof(e.PayloadId)} = {e.PayloadId}", true);
        _logger.Write($"{nameof(e.PackageOrContainerId)} = {e.PackageOrContainerId}", true);

        // base.OnCacheVerifyBegin(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheVerifyBegin)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has completed the verification of the acquired payload or container.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCacheVerifyComplete(object sender, CacheVerifyCompleteEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheVerifyComplete)} -------v");
        _logger.Write($"{nameof(e.PayloadId)} = {e.PayloadId}", true);
        _logger.Write($"{nameof(e.PackageOrContainerId)} = {e.PackageOrContainerId}", true);
        _logger.Write($"{nameof(e.Recommendation)} = {e.Recommendation}", true);

        // base.OnCacheVerifyComplete(sender, e);

        _logger.Write($"{nameof(e.Action)} = {e.Action}", true);
        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheVerifyComplete)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has progress verifying the payload or container.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCacheVerifyProgress(object sender, CacheVerifyProgressEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheVerifyProgress)} -------v");
        _logger.Write($"{nameof(e.Step)} = {e.Step}", true);
        LogCacheProgress(e);

        // base.OnCacheVerifyProgress(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheVerifyProgress)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has progress verifying the payload or container that was already in the package cache.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnCacheContainerOrPayloadVerifyProgress(object sender, CacheContainerOrPayloadVerifyProgressEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheContainerOrPayloadVerifyProgress)} -------v");
        LogCacheProgress(e);

        // base.OnCacheContainerOrPayloadVerifyProgress(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnCacheContainerOrPayloadVerifyProgress)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has begun installing a specific package.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnExecutePackageBegin(object sender, ExecutePackageBeginEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnExecutePackageBegin)} -------v");
        _logger.Write($"{nameof(e.PackageId)} = {e.PackageId}", true);
        _logger.Write($"{nameof(e.Action)} = {e.Action}", true);
        _logger.Write($"{nameof(e.UiLevel)} = {e.UiLevel}", true);
        _logger.Write($"{nameof(e.DisableExternalUiHandler)} = {e.DisableExternalUiHandler}", true);
        _logger.Write($"{nameof(e.ShouldExecute)} = {e.ShouldExecute}", true);

        // base.OnExecutePackageBegin(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnExecutePackageBegin)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has completed installing a specific package.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnExecutePackageComplete(object sender, ExecutePackageCompleteEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnExecutePackageComplete)} -------v");
        _logger.Write($"{nameof(e.Status)} = {ErrorHelper.HResultToMessage(e.Status)}", true);
        _logger.Write($"{nameof(e.PackageId)} = {e.PackageId}", true);
        _logger.Write($"{nameof(e.Restart)} = {e.Restart}", true);
        _logger.Write($"{nameof(e.Recommendation)} = {e.Recommendation}", true);

        // base.OnExecutePackageComplete(sender, e);

        _logger.Write($"{nameof(e.Action)} = {e.Action}", true);
        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnExecutePackageComplete)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine is about to pause Windows automatic updates.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnPauseAutomaticUpdatesBegin(object sender, PauseAutomaticUpdatesBeginEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnPauseAutomaticUpdatesBegin)} -------v");

        // base.OnPauseAutomaticUpdatesBegin(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnPauseAutomaticUpdatesBegin)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has completed pausing Windows automatic updates.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnPauseAutomaticUpdatesComplete(object sender, PauseAutomaticUpdatesCompleteEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnPauseAutomaticUpdatesComplete)} -------v");
        _logger.Write($"{nameof(e.Status)} = {ErrorHelper.HResultToMessage(e.Status)}", true);

        // base.OnPauseAutomaticUpdatesComplete(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnPauseAutomaticUpdatesComplete)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine is about to take a system restore point.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnSystemRestorePointBegin(object sender, SystemRestorePointBeginEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnSystemRestorePointBegin)} -------v");

        // base.OnSystemRestorePointBegin(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnSystemRestorePointBegin)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has completed taking a system restore point.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnSystemRestorePointComplete(object sender, SystemRestorePointCompleteEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnSystemRestorePointComplete)} -------v");
        _logger.Write($"{nameof(e.Status)} = {ErrorHelper.HResultToMessage(e.Status)}", true);

        // base.OnSystemRestorePointComplete(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnSystemRestorePointComplete)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine is about to start the elevated process.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnElevateBegin(object sender, ElevateBeginEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnElevateBegin)} -------v");

        // base.OnElevateBegin(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnElevateBegin)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has completed starting the elevated process.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnElevateComplete(object sender, ElevateCompleteEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnElevateComplete)} -------v");

        // base.OnElevateComplete(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnElevateComplete)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine is about to launch the pre-approved executable.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnLaunchApprovedExeBegin(object sender, LaunchApprovedExeBeginEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnLaunchApprovedExeBegin)} -------v");

        // base.OnLaunchApprovedExeBegin(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnLaunchApprovedExeBegin)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has completed launching the pre-approved executable.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnLaunchApprovedExeComplete(object sender, LaunchApprovedExeCompleteEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnLaunchApprovedExeComplete)} -------v");
        _logger.Write($"{nameof(e.Status)} = {ErrorHelper.HResultToMessage(e.Status)}", true);
        _logger.Write($"{nameof(e.ProcessId)} = {e.ProcessId}", true);

        // base.OnLaunchApprovedExeComplete(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnLaunchApprovedExeComplete)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has begun registering the location and visibility of the bundle.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnRegisterBegin(object sender, RegisterBeginEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnRegisterBegin)} -------v");
        _logger.Write($"{nameof(e.RecommendedRegistrationType)} = {e.RecommendedRegistrationType}", true);

        // base.OnRegisterBegin(sender, e);

        _logger.Write($"{nameof(e.RegistrationType)} = {e.RegistrationType}", true);
        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnRegisterBegin)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has completed registering the location and visibility of the bundle.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnRegisterComplete(object sender, RegisterCompleteEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnRegisterComplete)} -------v");
        _logger.Write($"{nameof(e.Status)} = {ErrorHelper.HResultToMessage(e.Status)}", true);

        // base.OnRegisterComplete(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnRegisterComplete)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine unregisters the bundle.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnUnregisterBegin(object sender, UnregisterBeginEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnUnregisterBegin)} -------v");
        _logger.Write($"{nameof(e.RecommendedRegistrationType)} = {e.RecommendedRegistrationType}", true);

        // base.OnUnregisterBegin(sender, e);

        _logger.Write($"{nameof(e.RegistrationType)} = {e.RegistrationType}", true);
        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnUnregisterBegin)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine unregistration is complete.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnUnregisterComplete(object sender, UnregisterCompleteEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnUnregisterComplete)} -------v");
        _logger.Write($"{nameof(e.Status)} = {ErrorHelper.HResultToMessage(e.Status)}", true);

        // base.OnUnregisterComplete(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnUnregisterComplete)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has changed progress for the bundle installation.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnProgress(object sender, ProgressEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnProgress)} -------v");
        _logger.Write($"{nameof(e.ProgressPercentage)} = {e.ProgressPercentage}", true);
        _logger.Write($"{nameof(e.OverallPercentage)} = {e.OverallPercentage}", true);

        // base.OnProgress(sender, e);

        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnProgress)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when Windows Installer sends an installation message.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnExecuteMsiMessage(object sender, ExecuteMsiMessageEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnExecuteMsiMessage)} -------v");
        _logger.Write($"{nameof(e.PackageId)} = {e.PackageId}", true);
        _logger.Write($"{nameof(e.MessageType)} = {e.MessageType}", true);
        _logger.Write($"{nameof(e.Message)} = {e.Message}", true);
        _logger.Write($"{nameof(e.UIHint)} = {e.UIHint}", true);
        _logger.Write($"{nameof(e.Recommendation)} = {e.Recommendation}", true);


        var data = e.Data?.Where(d => !string.IsNullOrWhiteSpace(d)).ToArray() ?? Array.Empty<string>();
        _logger.Write($"{nameof(e.Data)} (count = {data.Length})", true);
        foreach (var d in data)
          _logger.Write($"     {d}", true);

        // base.OnExecuteMsiMessage(sender, e);

        _logger.Write($"{nameof(e.Result)} = {e.Result}", true);
        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnExecuteMsiMessage)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when a package that spawned a process is cancelled.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnExecuteProcessCancel(object sender, ExecuteProcessCancelEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnExecuteProcessCancel)} -------v");
        _logger.Write($"{nameof(e.PackageId)} = {e.PackageId}", true);
        _logger.Write($"{nameof(e.ProcessId)} = {e.ProcessId}", true);
        _logger.Write($"{nameof(e.Recommendation)} = {e.Recommendation}", true);

        // base.OnExecuteProcessCancel(sender, e);

        _logger.Write($"{nameof(e.Action)} = {e.Action}", true);
        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnExecuteProcessCancel)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when a package sends a files in use installation message.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnExecuteFilesInUse(object sender, ExecuteFilesInUseEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnExecuteFilesInUse)} -------v");
        _logger.Write($"{nameof(e.PackageId)} = {e.PackageId}", true);
        _logger.Write($"{nameof(e.Source)} = {e.Source}", true);
        _logger.Write($"{nameof(e.Recommendation)} = {e.Recommendation}", true);

        var files = e.Files?.Where(f => !string.IsNullOrWhiteSpace(f)).ToArray() ?? Array.Empty<string>();
        _logger.Write($"{nameof(e.Files)} (count = {files.Length})", true);
        foreach (var file in files)
          _logger.Write($"    {file}", true);

        // base.OnExecuteFilesInUse(sender, e);

        _logger.Write($"{nameof(e.Result)} = {e.Result}", true);
        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnExecuteFilesInUse)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    /// <summary>
    ///   Fired when the engine has encountered an error.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="PhaseException"></exception>
    public override void OnError(object sender, ErrorEventArgs e)
    {
      try
      {
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnError)} -------v");
        _logger.Write($"{nameof(e.UIHint)} = {e.UIHint}", true);
        _logger.Write($"{nameof(e.Recommendation)} = {e.Recommendation}", true);

        base.OnError(sender, e);

        _logger.Write($"{nameof(e.Result)} = {e.Result}", true);
        _logger.Write($"{nameof(e.HResult)} = {ErrorHelper.HResultToMessage(e.HResult)}", true);
        _logger.Write($"{nameof(ApplyPhase)}: {nameof(OnError)} -------^");
      }
      catch (PhaseException)
      {
        throw;
      }
      catch (Exception ex)
      {
        _logger.Write(ex);
        throw new PhaseException(ex);
      }
    }

    private void LogCacheProgress(CacheProgressBaseEventArgs e)
    {
      _logger.Write($"{nameof(e.PayloadId)} = {e.PayloadId}", true);
      _logger.Write($"{nameof(e.PackageOrContainerId)} = {e.PackageOrContainerId}", true);
      _logger.Write($"{nameof(e.Total)} = {e.Total}", true);
      _logger.Write($"{nameof(e.Progress)} = {e.Progress}", true);
      _logger.Write($"{nameof(e.OverallPercentage)} = {e.OverallPercentage}", true);
    }
  }
}