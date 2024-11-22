using Common.WPF;
using FractalFlair.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace FractalFlair.ViewModels;

public class ShellViewModel : ViewModelBase
{
  public ShellViewModel(INotificationService exService, INotificationService uiNotifier)
  {
    CalculatorVms = new ObservableCollection<CalculatorViewModel> { new(exService, uiNotifier) };
    _selectedCalculatorVm = CalculatorVms.First();
  }

  public ObservableCollection<CalculatorViewModel> CalculatorVms { get; }

  private void ResetSelectedCalculatorView()
  {
    if (_selectedCalculatorVm.ResetViewCommand.CanExecute())
      _selectedCalculatorVm.ResetViewCommand.Execute();
  }

  #region SelectedCalculatorVm

  private CalculatorViewModel _selectedCalculatorVm;

  public CalculatorViewModel SelectedCalculatorVm
  {
    get => _selectedCalculatorVm;
    set
    {
      if (_selectedCalculatorVm != value)
      {
        _selectedCalculatorVm = value;
        OnPropertyChanged();
        ResetSelectedCalculatorView();
      }
    }
  }

  #endregion SelectedCalculatorVm
}