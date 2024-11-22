using System;

namespace Setup.BA.Util
{
  /// <summary>
  ///   Exposes additional formatting for <see cref="Exception" /> using
  ///   the Command design pattern
  /// </summary>
  public interface IExceptionFormatCommand
  {
    /// <summary>
    ///   Provides text that will be appended to the Additional Details
    ///   of an exception report
    /// </summary>
    /// <param name="ex">
    ///   The exception to handle
    /// </param>
    /// <returns>
    ///   Returns null if the supplied exception is not supported, otherwise
    ///   returns the text to append to the Addition Details section of an
    ///   exception report
    /// </returns>
    string AddExceptionDetails(Exception ex);

    /// <summary>
    ///   An optional implementation which formats the text differently when reported
    ///   as an inner exception
    /// </summary>
    /// <param name="ex">
    ///   The exception to handle
    /// </param>
    /// <returns>
    ///   Returns null if there's no need to format details differently for inner exceptions
    /// </returns>
    string AddInnerExceptionDetails(Exception ex);
  }
}