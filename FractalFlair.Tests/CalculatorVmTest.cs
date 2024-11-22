// Ignore Spelling: Vm Svc

using Common.Exceptions;
using FractalFlair.Models;
using FractalFlair.ViewModels;
using JetBrains.Annotations;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace FractalFlair.Tests;

[TestClass]
public class CalculatorVmTest
{
  [UsedImplicitly]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  public TestContext TestContext { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

  [TestMethod]
  public async Task VmTest()
  {
    var file = Path.GetFullPath(Path.Combine(TestContext.TestResultsDirectory ?? string.Empty, "..", "..", "fractal.png"));
    if (File.Exists(file))
      File.Delete(file);

    var notificationSvc = new NotificationService();
    notificationSvc.ExceptionRaised += ExSvc_ExceptionRaised;

    var vm = new CalculatorViewModel(notificationSvc, notificationSvc)
    {
      Width = 400,
      Height = 400,
      Iterations = 50
    };

    try
    {
      await vm.CalculateCommand.ExecuteAsync();
    }
    catch (OperationCanceledException)
    {
      vm.CancelCommand.Execute();
      throw;
    }
    finally
    {
      notificationSvc.ExceptionRaised -= ExSvc_ExceptionRaised;
    }

    Assert.IsNotNull(vm.Fractal, "No fractal was produced");
    Assert.IsNotNull(vm.Bitmap, "No image was produced");
    vm.Bitmap.Save(file, ImageFormat.Png);
    Debug.Write($"Saved test result as {file}");
  }

  private void ExSvc_ExceptionRaised(object? sender, ExceptionEventArgs e)
  {
    Assert.Fail($"Test failed with an exc exception\r\n{e.Exception.FormattedMessage()}");
  }
}