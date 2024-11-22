using Setup.BA.Models;
using Setup.BA.Util;

namespace Setup.BA.ViewModels
{
  internal class CancelViewModel : ViewModelBase
  {
    private readonly Model _model;

    public CancelViewModel(Model model)
    {
      _model = model;
      CancelCommand = new DelegateCommand(Cancel, CanCancel);
    }

    public IDelegateCommand CancelCommand { get; }

    private void Cancel()
    {
      _model.State.CancelRequested = true;
    }

    private bool CanCancel()
    {
      return !_model.State.CancelRequested && (_model.State.BaStatus == BaStatus.Planning || _model.State.BaStatus == BaStatus.Applying);
    }
  }
}