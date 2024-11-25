﻿using Setup.BA.Util;
using System;
using WixToolset.BootstrapperApplicationApi;

namespace Setup.BA
{
  internal class Program
  {
    private static int Main()
    {
      int exitCode;
      try
      {
        var application = new BootstrapperApp();
        ManagedBootstrapperApplication.Run(application);
        exitCode = application.ExitCode;
      }
      catch (Exception ex)
      {
        exitCode = ErrorHelper.HResultToWin32(ex.HResult);
      }

      return exitCode;
    }
  }
}