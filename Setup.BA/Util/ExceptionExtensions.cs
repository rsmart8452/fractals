using JetBrains.Annotations;
using System;
using System.Text;
using System.Threading;

namespace Setup.BA.Util
{
  /// <summary>
  ///   Provides extension methods for <see cref="Exception" />
  /// </summary>
  public static class ExceptionExtensions
  {
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private static IExceptionFormatCommand _registered;

    /// <summary>
    ///   Allows registering a default format command to be used for.
    ///   <see cref="FormattedMessage" /> unless an override is provided. This is not
    ///   thread safe and is intended to be called only once during application startup.
    /// </summary>
    /// <param name="command">
    ///   The command to register
    /// </param>
    [UsedImplicitly]
    public static void RegisterFormatCommand(IExceptionFormatCommand command)
    {
      _semaphore.Wait();
      try
      {
        _registered = command;
      }
      finally
      {
        _semaphore.Release();
      }
    }

    /// <summary>
    ///   Formats the given exception for troubleshooting.
    /// </summary>
    /// <param name="ex">
    ///   Exception to format.
    /// </param>
    /// <param name="command">
    ///   An optional Command which provides additional text for specific exceptions.
    /// </param>
    /// <returns>
    ///   Returns a verbose description of the exception, any data associated with it,
    ///   it's stack trace, and details on all inner exceptions.
    /// </returns>
    [UsedImplicitly]
    public static string FormattedMessage(this Exception ex, IExceptionFormatCommand command = null)
    {
      var cmd = command ?? _registered;

      var messageSb = new StringBuilder();
      messageSb.AppendLine(ex.Message);
      messageSb.AppendLine($"  ({ex.GetType().FullName})");
      messageSb.AppendLine();
      messageSb.Append("Source: ");
      messageSb.AppendLine(ex.Source ?? "Source unavailable");

      if (ex.Data.Count > 0)
      {
        messageSb.AppendLine();
        messageSb.AppendLine("Associated Data:");
        messageSb.AppendLine("------------------------------");

        foreach (var key in ex.Data.Keys)
        {
          try
          {
            var k = key?.ToString();
            if (string.IsNullOrEmpty(k))
              continue;

            var v = ex.Data[key]?.ToString() ?? "!!! Value is null !!!";
            messageSb.AppendLine($"{k,-10} - {v}");
          }
          catch
          {
            // ignore
          }
        }
      }

      messageSb.AppendLine();
      messageSb.AppendLine("Stack Trace:");
      messageSb.AppendLine("------------------------------");
      messageSb.AppendLine(ex.StackTrace ?? "Stack trace unavailable");

      // A partial class can be defined to provide additional details about exceptions
      // that are specific to the application or .NET framework. Examples of what can be
      // provided are handling specifics of WCF and Entity Framework exceptions, which can
      // reveal details like the reason a WCF connection failed or the specifics concerning
      // the table, column, and reason EF mapping fails.
      var details = cmd?.AddExceptionDetails(ex);
      if (!string.IsNullOrEmpty(details))
      {
        messageSb.AppendLine();
        messageSb.AppendLine("==============================");
        messageSb.AppendLine("Details:");
        messageSb.AppendLine("------------------------------");
        messageSb.Append(details);
      }

      // A nice hook to handle AggregateException, which is specific to .NET 4+
      var innerDetails = cmd?.AddInnerExceptionDetails(ex);
      if (!string.IsNullOrEmpty(innerDetails))
        messageSb.AppendLine(innerDetails);
      else if (ex.InnerException != null)
      {
        messageSb.AppendLine();
        messageSb.AppendLine();
        messageSb.AppendLine("Inner Exception:");
        messageSb.AppendLine("------------------------------");
        messageSb.Append(ex.InnerException.FormattedMessage());
      }

      return messageSb.ToString();
    }

    /// <summary>
    ///   Reports the Message from the innermost exception
    /// </summary>
    /// <param name="ex">
    ///   Exception to analyze
    /// </param>
    /// <returns>
    ///   Returns the innermost message of the exception. If the exception is an <see cref="AggregateException" />
    ///   then all relevant inner messages will be provided.
    /// </returns>
    [UsedImplicitly]
    public static string InnerMessage(this Exception ex)
    {
      if (!(ex is AggregateException agEx))
        return ex.InnerException?.InnerMessage() ?? ex.Message;

      var flatEx = agEx.Flatten();
      if (flatEx.InnerExceptions.Count > 1)
      {
        var sb = new StringBuilder();
        sb.Append(flatEx.Message);
        foreach (var innerEx in flatEx.InnerExceptions)
          sb.Append($"\r\n{innerEx.InnerMessage()}");

        return sb.ToString();
      }

      return flatEx.InnerException?.InnerMessage() ?? flatEx.Message;
    }
  }
}