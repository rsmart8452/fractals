using System.Windows;

namespace Setup.BA.Views
{
  public partial class ProgressView
  {
    public ProgressView()
    {
      InitializeComponent();
    }

    public bool IsProgressVisible
    {
      get => (bool)GetValue(IsProgressVisibleProperty);
      set => SetValue(IsProgressVisibleProperty, value);
    }

    #region ${Name}

    #region ${Name}

    public static readonly DependencyProperty IsProgressVisibleProperty = DependencyProperty
      .Register(
        nameof(IsProgressVisible),
        typeof(bool),
        typeof(ProgressView),
        new PropertyMetadata(default(bool)));

    #endregion

    #endregion
  }
}