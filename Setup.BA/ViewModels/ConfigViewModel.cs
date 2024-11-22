using Setup.BA.Models;
using Setup.BA.Util;

namespace Setup.BA.ViewModels
{
  internal class ConfigViewModel : ViewModelBase
  {
    private readonly Model _model;
    //private bool _isInstalled;

    private string _installLocation;

    public ConfigViewModel(Model model)
    {
      _model = model;
      _installLocation = _model.Engine.FormatString(Constants.PerMachineLocation);
    }

    public string InstallLocation
    {
      get => _installLocation;
      set
      {
        if (_installLocation == value)
          return;

        _installLocation = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    ///   Executed by <see cref="ShellViewModel" /> after the detect phase has completed, but before the plan phase starts
    /// </summary>
    public void AfterDetect()
    { }
  }
}