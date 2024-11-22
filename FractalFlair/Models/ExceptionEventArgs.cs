using System;

namespace FractalFlair.Models;

public class ExceptionEventArgs : EventArgs
{
  public ExceptionEventArgs(Exception exception)
  {
    Exception = exception;
  }

  public Exception Exception { get; }
}