using System;
using System.Linq;

namespace FractalFlair.Models;

public static class ExceptionExtensions
{
  public static bool IsCancel(this Exception ex)
  {
    if (ex is OperationCanceledException)
      return true;

    if (ex is AggregateException agEx)
    {
      var flatEx = agEx.Flatten();
      return flatEx.InnerExceptions.Any(e => e is OperationCanceledException);
    }

    return false;
  }
}