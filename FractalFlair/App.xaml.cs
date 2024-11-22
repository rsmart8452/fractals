// Ignore Spelling: App

using Common.Exceptions;
using FractalFlair.Models;
using FractalFlair.ViewModels;
using FractalFlair.Views;
using FractalFlair.Views.Controls;
using System;
using System.Windows;

namespace FractalFlair;

public partial class App
{
  private static void NotificationService_ExceptionRaised(object? sender, ExceptionEventArgs e)
  {
    HandleException(e.Exception);
  }

  private static void HandleException(Exception ex)
  {
#if DEBUG
    var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    var errFile = System.IO.Path.Combine(path, "FractalFlair_error.txt");
    System.IO.File.WriteAllText(errFile, ex.FormattedMessage());
    System.Diagnostics.Process.Start(errFile);
#else
    MessageBox.Show(ex.InnerMessage(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
  }

  private void Application_Startup(object sender, StartupEventArgs e)
  {
    try
    {
      INotificationService notificationService = new NotificationService();

      // Dispatches events to the UI thread
      INotificationService uiNotifier = new UiNotificationService(notificationService, Dispatcher);
      uiNotifier.ExceptionRaised += NotificationService_ExceptionRaised;

      var vm = new ShellViewModel(notificationService, uiNotifier);
      var shell = new ShellView { DataContext = vm };
      shell.AssignNotifiers(notificationService, uiNotifier);
      shell.Show();
    }
    catch (Exception ex)
    {
      HandleException(ex);
    }
  }
}