using FractalFlair.Models;

namespace FractalFlair.Views;

public partial class ShellView
{
  public ShellView()
  {
    InitializeComponent();
  }

  public void AssignNotifiers(INotificationService zoomNotifier, INotificationService resetNotifier)
  {
    if (CalcView == null)
      return;

    CalcView.ZoomNotifier = zoomNotifier;
    CalcView.ResetNotifier = resetNotifier;
  }
}