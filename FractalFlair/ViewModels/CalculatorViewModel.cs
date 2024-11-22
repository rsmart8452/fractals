// Ignore Spelling: Vm

using Common.WPF;
using Common.WPF.Commands;
using FractalFlair.Models;
using FractalFlair.Views.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ColorMap = FractalFlair.Models.ColorMap;

namespace FractalFlair.ViewModels;

public class CalculatorViewModel : ViewModelBase
{
  private readonly IAsyncCommand _generateAnimatedJuliaCommand;

  /// <summary>
  ///   Service that this VM can use to notify the UI of events that occur
  /// </summary>
  private readonly INotificationService _notificationService;

  /// <summary>
  ///   Service that will notify the VM of events that occur. This notification will always be triggered on the UI thread.
  /// </summary>
  private readonly INotificationService _uiNotifier;

  public CalculatorViewModel(INotificationService exService, INotificationService uiNotifier)
  {
    _generateAnimatedJuliaCommand = new AsyncCommand(GenerateAnimatedJuliaAsync, null, OnCalculateException);
    _notificationService = exService;
    _notificationService.UiZoomComplete += NotificationService_UiZoomComplete;
    _uiNotifier = uiNotifier;
    _uiNotifier.JuliaAnimationRequested += UiNotifier_JuliaAnimationRequested;
    InitializeSettings();
    _requestedWidth = _width;
    _requestedHeight = _height;
    _animatedJuliaFrames = 20;
    _showCrossHair = true;
    var maps = ColorMaps.GetAvailableMaps();
    FractalTypes = new ObservableCollection<string>(FractalType.AllTypes);
    _selectedFractalType = FractalTypes.First();
    AvailableColorMaps = new ObservableCollection<ColorMap>(maps);
    _selectedColorMap = AvailableColorMaps.First();
    DrawJuliaOriginLineCommand = new DelegateCommand(DrawJuliaOriginLine, CanDrawJuliaOriginLine);
    DrawJuliaOriginLineCancelCommand = new DelegateCommand(DrawJuliaOriginLineCancel, CanDrawJuliaOriginLineCancel);
    ApplyColorMapCommand = new AsyncCommand(ApplyColorMapAsync, CanApplyColorMap);
    CalculateCommand = new AsyncCommand(CalculateAsync, CanCalculate, OnCalculateException);
    CalculateJuliaOriginCommand = new AsyncCommand(CalculateJuliaOriginAsync, CanCalculateJuliaOrigin);
    SaveCommand = new DelegateCommand(Save, CanSave);
    CancelCommand = new DelegateCommand(Cancel, CanCancel);
    ResetFractalCommand = new DelegateCommand(ResetFractal, CanResetFractal);
    ResetViewCommand = new DelegateCommand(ResetView, CanResetView);
    _combinedZoomFactorFormatted = FormatZoomFactor();
  }

  public IAsyncCommand ApplyColorMapCommand { get; }
  public IDelegateCommand DrawJuliaOriginLineCommand { get; }
  public IDelegateCommand DrawJuliaOriginLineCancelCommand { get; }
  public IAsyncCommand CalculateCommand { get; }
  public IAsyncCommand CalculateJuliaOriginCommand { get; }
  public IDelegateCommand SaveCommand { get; }
  public IDelegateCommand CancelCommand { get; }
  public IDelegateCommand ResetViewCommand { get; }
  public IDelegateCommand ResetFractalCommand { get; }
  public ObservableCollection<ColorMap> AvailableColorMaps { get; }
  public ObservableCollection<string> FractalTypes { get; }
  public Bitmap? Bitmap => Fractal?.Bitmap;
  public bool IsMandelbrotFractal => Fractal?.Type == FractalType.Mandelbrot;

  protected override IEnumerable<Err> GetVmErrors()
  {
    var errors = new List<Err>();
    if (CombinedZoomFactor > 80000000000000d)
    {
      errors.Add(new Err(nameof(CombinedZoomFactor), "Zoom may be too high"));
      errors.Add(new Err(nameof(CombinedZoomFactorFormatted), "Zoom may be too high"));
    }

    if (AnimatedJuliaFrames < 2)
      errors.Add(new Err(nameof(AnimatedJuliaFrames), "Must be greater than 1"));

    return errors;
  }

  private string FormatZoomFactor()
  {
    if (Math.Abs(CombinedZoomFactor) > 0.001 && Math.Abs(CombinedZoomFactor) <= 10d)
      return $"{CombinedZoomFactor:#,##0.0}";

    return $"{CombinedZoomFactor:#,##0}";
  }

  private void Save()
  {
    if (!Directory.Exists("Images"))
      Directory.CreateDirectory("Images");

    var filePath = Path.Combine("Images", $"{Path.GetRandomFileName()}.png");
    Bitmap?.Save(filePath, ImageFormat.Png);
  }

  private bool CanSave()
  {
    return !IsBusy && Bitmap != null;
  }

  private void ResetFractal()
  {
    Fractal = null;
    CombinedZoomFactor = 0d;
  }

  private bool CanResetFractal()
  {
    return !IsBusy;
  }

  private async Task CalculateJuliaOriginAsync()
  {
    if (Fractal == null)
      return;

    IsBusy = true;
    Cts = new CancellationTokenSource();
    CancelCommand.RaiseCanExecuteChanged();
    try
    {
      Percent = 0d;
      // Grab settings before UI is reset and, in turn, resets these settings
      var width = RequestedWidth;
      var height = RequestedHeight;
      var centerRatioX = CenterRatioX;
      var centerRatioY = CenterRatioY;

      CalculateJuliaOriginCommand.RaiseCanExecuteChanged();
      _generateAnimatedJuliaCommand.RaiseCanExecuteChanged();
      var progress = new Progress<FractalProgress>(OnProgress);
      var calculator = BuildCalculator(progress, FractalType.Julia);
      Width = width;
      Height = height;

      var token1 = Cts.Token;
      var fractal = await calculator.CalculateJuliaFromOriginAsync(
          Fractal,
          width,
          height,
          centerRatioX,
          centerRatioY,
          Iterations,
          token1)
        .ConfigureAwait(true);

      SelectedFractalType = FractalTypes.First(t => t == FractalType.Julia);
      Fractal = fractal;
    }
    catch (Exception ex) when (ex.IsCancel())
    { }
    catch (Exception ex)
    {
      _notificationService.HandleException(ex);
    }
    finally
    {
      Cts = null;
      Percent = 0d;
      IsBusy = false;
      OnCalculateComplete();
    }
  }

  private bool CanCalculateJuliaOrigin()
  {
    return !IsBusy && IsMandelbrotFractal;
  }

  private void StartAnimateJulia()
  {
    if (Fractal is not { Type: FractalType.Mandelbrot })
      return;

    var start = Fractal.FindPoint(JuliaOriginRatioStartX, JuliaOriginRatioStartY);
    var end = Fractal.FindPoint(JuliaOriginRatioEndX, JuliaOriginRatioEndY);

    // Cue triggering _generateAnimatedJuliaCommand on the UI thread
    _uiNotifier.RequestJuliaAnimation(start, end, AnimatedJuliaFrames);
  }

  private async Task GenerateAnimatedJuliaAsync()
  {
    if (Fractal == null)
      return;

    IsBusy = true;
    Cts = new CancellationTokenSource();
    CancelCommand.RaiseCanExecuteChanged();
    try
    {
      Percent = 0d;
      var progress = new Progress<FractalProgress>(OnProgress);
      var calculator = BuildCalculator(progress, FractalType.Julia);

      var startOrigin = Fractal.FindPoint(JuliaOriginRatioStartX, JuliaOriginRatioStartY);
      var endOrigin = Fractal.FindPoint(JuliaOriginRatioEndX, JuliaOriginRatioEndY);

      var token1 = Cts.Token;

      var (fractal, bitmapImage) = await calculator
        .AnimateJuliaAsync(Width, Height, startOrigin, endOrigin, AnimatedJuliaFrames, Iterations, token1)
        .ConfigureAwait(true);

      SelectedFractalType = FractalTypes.First(t => t == FractalType.Julia);
      Fractal = fractal;
      AnimatedJuliaImage = bitmapImage;
      ShowAnimatedJulia = true;
    }
    catch (Exception ex) when (ex.IsCancel())
    { }
    catch (Exception ex)
    {
      _notificationService.HandleException(ex);
    }
    finally
    {
      Cts = null;
      Percent = 0d;
      IsBusy = false;
      OnCalculateComplete();
    }
  }

  private void UiNotifier_JuliaAnimationRequested(object? sender, EventArgs e)
  {
    _generateAnimatedJuliaCommand.Execute(null);
  }

  private void NotificationService_UiZoomComplete(object? sender, ZoomEventArgs e)
  {
    UpdateCombinedZoomFactor(e.ZoomFactor);
  }

  private void UpdateCombinedZoomFactor(double zoomFactor = 1d)
  {
    if (Fractal != null)
      CombinedZoomFactor = Fractal.ZoomFactor * zoomFactor;
  }

  private async Task CalculateAsync()
  {
    IsBusy = true;
    Cts = new CancellationTokenSource();
    CancelCommand.RaiseCanExecuteChanged();
    try
    {
      Percent = 0d;
      var progress = new Progress<FractalProgress>(OnProgress);
      var calculator = BuildCalculator(progress, SelectedFractalType);

      var token1 = Cts.Token;
      Fractal fractal;

      if (Fractal == null)
      {
        fractal = await calculator
          .CalculateAsync(Width, Height, Iterations, token1)
          .ConfigureAwait(true);
      }
      else
      {
        fractal = await calculator
          .CalculateAsync(
            Fractal,
            Width,
            Height,
            CenterRatioX,
            CenterRatioY,
            Scale,
            Iterations,
            UseHighPrecision,
            token1)
          .ConfigureAwait(true);

        fractal.ZoomFactor = Fractal.ZoomFactor * Scale;
      }

      Fractal = fractal;
    }
    catch (Exception ex) when (ex.IsCancel())
    { }
    catch (Exception ex)
    {
      _notificationService.HandleException(ex);
    }
    finally
    {
      Cts = null;
      Percent = 0d;
      IsBusy = false;
      OnCalculateComplete();
    }
  }

  private void OnCalculateException(Exception ex)
  {
    if (ex.IsCancel())
    {
      CancelCommand.RaiseCanExecuteChanged();
      return;
    }

    _notificationService.HandleException(ex);
  }

  private bool CanCalculate()
  {
    return !IsBusy;
  }

  private void Cancel()
  {
    Cts?.Cancel();
    CancelCommand.RaiseCanExecuteChanged();
  }

  private bool CanCancel()
  {
    return IsBusy && Cts is { IsCancellationRequested: false };
  }

  private void ResetView()
  {
    _notificationService.RequestZoomScaleReset();
  }

  private bool CanResetView()
  {
    return !IsBusy && BitmapSource != null;
  }

  private async Task ApplyColorMapAsync()
  {
    if (Fractal == null)
      return;

    IsBusy = true;
    Percent = 0d;
    Cts = new CancellationTokenSource();
    try
    {
      var token = Cts.Token;
      var progress = new Progress<FractalProgress>(OnProgress);
      var map = SelectedColorMap;
      var bitmap = await Task
        .Run(() => ApplyColorMap(Fractal, map, progress, token), token)
        .ConfigureAwait(true);

      if (bitmap != null)
      {
        Fractal.Bitmap = bitmap;
        BitmapSource = Fractal.Bitmap.ToBitmapImage();
        OnPropertyChanged(nameof(Bitmap));
      }
    }
    finally
    {
      Cts = null;
      Percent = 0d;
      IsBusy = false;
    }
  }

  private Bitmap? ApplyColorMap(Fractal fractal, ColorMap map, IProgress<FractalProgress> progress, CancellationToken token)
  {
    try
    {
      var colorMapService = new ColorMapService();
      token.ThrowIfCancellationRequested();
      return colorMapService.GenerateBitmapFromFractal(map, fractal, progress, token);
    }
    catch (Exception ex) when (ex.IsCancel())
    { }
    catch (Exception ex)
    {
      _notificationService.HandleException(ex);
    }

    return null;
  }

  private bool CanApplyColorMap()
  {
    return !IsBusy && Fractal != null;
  }

  private void DrawJuliaOriginLine()
  {
    IsDrawingJuliaOriginLine = true;
  }

  private bool CanDrawJuliaOriginLine()
  {
    return Fractal is { Type: FractalType.Mandelbrot } && !IsDrawingJuliaOriginLine;
  }

  private void DrawJuliaOriginLineCancel()
  {
    _isDrawingJuliaOriginLine = false;
    OnPropertyChanged(nameof(IsDrawingJuliaOriginLine));
    DrawJuliaOriginLineCommand.RaiseCanExecuteChanged();
    DrawJuliaOriginLineCancelCommand.RaiseCanExecuteChanged();
  }

  private bool CanDrawJuliaOriginLineCancel()
  {
    return IsDrawingJuliaOriginLine;
  }

  private FractalCalculator BuildCalculator(IProgress<FractalProgress> progress, string fractalType)
  {
    var calculator = FractalType.GetCalculator(fractalType);
    return new FractalCalculator(calculator, SelectedColorMap, progress);
  }

  private void InitializeBasicSettings(bool notifyUi = false)
  {
    _width = 800;
    _height = 800;
    _iterations = _selectedFractalType switch
    {
      FractalType.Julia => 2000,
      FractalType.BurningShip => 5000,
      _ => 200
    };

    if (notifyUi)
      NotifyBasicSettingsChanged();
  }

  private void NotifyBasicSettingsChanged()
  {
    OnPropertyChanged(nameof(Width));
    OnPropertyChanged(nameof(Height));
    OnPropertyChanged(nameof(Iterations));
  }

  private void InitializeAdvancedSettings(bool notifyUi = false)
  {
    _zoomCenterX = 0d;
    _zoomCenterY = 0d;
    _centerRatioX = 0.5d;
    _centerRatioY = 0.5d;
    _scale = 1d;

    if (notifyUi)
      NotifyAdvancedSettingsChanged();
  }

  private void NotifyAdvancedSettingsChanged()
  {
    OnPropertyChanged(nameof(ZoomCenterX));
    OnPropertyChanged(nameof(ZoomCenterY));
    OnPropertyChanged(nameof(CenterRatioX));
    OnPropertyChanged(nameof(CenterRatioY));
    OnPropertyChanged(nameof(Scale));
    RaiseCommandNotifications();
    _notificationService.RequestZoomScaleReset();
  }

  private void RaiseCommandNotifications()
  {
    CalculateCommand.RaiseCanExecuteChanged();
    SaveCommand.RaiseCanExecuteChanged();
    CancelCommand.RaiseCanExecuteChanged();
    ApplyColorMapCommand.RaiseCanExecuteChanged();
    ResetFractalCommand.RaiseCanExecuteChanged();
    CalculateJuliaOriginCommand.RaiseCanExecuteChanged();
    _generateAnimatedJuliaCommand.RaiseCanExecuteChanged();
    DrawJuliaOriginLineCommand.RaiseCanExecuteChanged();
    ResetViewCommand.RaiseCanExecuteChanged();
  }

  private void InitializeSettings(bool notifyUi = false)
  {
    InitializeBasicSettings(notifyUi);
    InitializeAdvancedSettings(notifyUi);
  }

  private void OnFractalChanged()
  {
    try
    {
      if (_fractal == null)
        InitializeAdvancedSettings(true);
      else
      {
        _iterations = _fractal.MaxIterations;
        _width = _fractal.Width;
        _height = _fractal.Height;
        _zoomCenterX = _width / 2d;
        _zoomCenterY = _height / 2d;
        _scale = 1d;
      }

      UpdateCombinedZoomFactor();
      NotifyBasicSettingsChanged();
      NotifyAdvancedSettingsChanged();
    }
    catch (Exception ex)
    {
      _notificationService.HandleException(ex);
    }
  }

  private void OnCalculateComplete()
  {
    ResetViewCommand.RaiseCanExecuteChanged();
    ApplyColorMapCommand.RaiseCanExecuteChanged();
  }

  private void OnProgress(FractalProgress report)
  {
    Percent = report.TotalProgress * 100d;
  }

  #region Cts

  private CancellationTokenSource? _cts;

  private CancellationTokenSource? Cts
  {
    get => _cts;
    set
    {
      if (_cts == value)
        return;

      _cts?.Dispose();
      _cts = value;
    }
  }

  #endregion Cts

  #region SelectedFractalType

  private string _selectedFractalType;

  public string SelectedFractalType
  {
    get => _selectedFractalType;
    set
    {
      if (_selectedFractalType == value)
        return;

      _selectedFractalType = value;
      OnPropertyChanged();
      Fractal = null;
      InitializeSettings(true);
      RaiseCommandNotifications();
    }
  }

  #endregion SelectedFractalType

  #region Fractal

  private Fractal? _fractal;

  public Fractal? Fractal
  {
    get => _fractal;
    private set
    {
      if (_fractal == value)
        return;

      _fractal?.Dispose();
      _fractal = value;
      OnPropertyChanged();
      OnPropertyChanged(nameof(Bitmap));
      OnPropertyChanged(nameof(IsMandelbrotFractal));
      BitmapSource = _fractal?.Bitmap.ToBitmapImage();
      ShowAnimatedJulia = false;
      AnimatedJuliaImage = null;
      OnFractalChanged();
      RaiseCommandNotifications();
    }
  }

  #endregion Fractal

  #region BitmapSource

  private BitmapSource? _bitmapSource;

  public BitmapSource? BitmapSource
  {
    get => _bitmapSource;
    set
    {
      if (_bitmapSource == value)
        return;

      _bitmapSource = value;
      OnPropertyChanged();
    }
  }

  #endregion BitmapSource

  #region ShowCrosshair

  private bool _showCrossHair;

  public bool ShowCrossHair
  {
    get => _showCrossHair;
    set
    {
      if (_showCrossHair == value)
        return;

      _showCrossHair = value;
      OnPropertyChanged();
    }
  }

  #endregion ShowCrosshair

  #region UseHighPrecision

  private bool _useHighPrecision;

  public bool UseHighPrecision
  {
    get => _useHighPrecision;
    set
    {
      if (_useHighPrecision == value)
        return;

      _useHighPrecision = value;
      OnPropertyChanged();
    }
  }

  #endregion UseHighPrecision

  #region Iterations

  private int _iterations;

  public int Iterations
  {
    get => _iterations;
    set
    {
      if (_iterations != value)
      {
        _iterations = value;
        OnPropertyChanged();
      }
    }
  }

  #endregion Iterations

  #region Height

  private int _height;

  public int Height
  {
    get => _height;
    set
    {
      if (_height != value)
      {
        _height = value;
        _zoomedHeight = _height;
        _zoomCenterY = _height / 2d;
        OnPropertyChanged();
        OnPropertyChanged(nameof(ZoomCenterY));
        OnPropertyChanged(nameof(ZoomedHeight));
      }
    }
  }

  #endregion Height

  #region Width

  private int _width;

  public int Width
  {
    get => _width;
    set
    {
      if (_width == value)
        return;

      _width = value;
      _zoomCenterX = _width / 2d;
      _zoomedWidth = _width;
      OnPropertyChanged();
      OnPropertyChanged(nameof(ZoomCenterX));
      OnPropertyChanged(nameof(ZoomedWidth));
    }
  }

  #endregion Width

  #region ZoomCenterY

  private double _zoomCenterY;

  public double ZoomCenterY
  {
    get => _zoomCenterY;
    set
    {
      _zoomCenterY = value;
      OnPropertyChanged();
    }
  }

  #endregion ZoomCenterY

  #region ZoomCenterX

  private double _zoomCenterX;

  public double ZoomCenterX
  {
    get => _zoomCenterX;
    set
    {
      _zoomCenterX = value;
      OnPropertyChanged();
    }
  }

  #endregion ZoomCenterX

  #region SelectedColorMap

  private ColorMap _selectedColorMap;

  public ColorMap SelectedColorMap
  {
    get => _selectedColorMap;
    set
    {
      if (_selectedColorMap == value)
        return;

      _selectedColorMap = value;
      OnPropertyChanged();
    }
  }

  #endregion SelectedColorMap

  #region Scale

  private double _scale;

  public double Scale
  {
    get => _scale;
    set
    {
      _scale = value;
      if (_fractal == null)
        _combinedZoomFactor = _scale;
      else
        _combinedZoomFactor = _scale * _fractal.ZoomFactor;

      OnPropertyChanged();
      OnPropertyChanged(nameof(CombinedZoomFactor));
    }
  }

  #endregion Scale

  #region ZoomedWidth

  private double _zoomedWidth;

  public double ZoomedWidth
  {
    get => _zoomedWidth;
    set
    {
      if (Math.Abs(_zoomedWidth - value) < 0.0000000000001d)
        return;

      _zoomedWidth = value;
      OnPropertyChanged();
    }
  }

  #endregion ZoomedWidth

  #region ZoomedHeight

  private double _zoomedHeight;

  public double ZoomedHeight
  {
    get => _zoomedHeight;
    set
    {
      if (Math.Abs(_zoomedHeight - value) < 0.0000000000001d)
        return;

      _zoomedHeight = value;
      OnPropertyChanged();
    }
  }

  #endregion ZoomedHeight

  #region RequestedWidth

  private int _requestedWidth;

  public int RequestedWidth
  {
    get => _requestedWidth;
    set
    {
      if (_requestedWidth == value)
        return;

      _requestedWidth = value;
      OnPropertyChanged();
    }
  }

  #endregion RequestedWidth

  #region RequestedHeight

  private int _requestedHeight;

  public int RequestedHeight
  {
    get => _requestedHeight;
    set
    {
      if (_requestedHeight == value)
        return;

      _requestedHeight = value;
      OnPropertyChanged();
    }
  }

  #endregion RequestedHeight

  #region CenterRatioX

  private double _centerRatioX;

  public double CenterRatioX
  {
    get => _centerRatioX;
    set
    {
      _centerRatioX = value;
      OnPropertyChanged();
    }
  }

  #endregion CenterRatioX

  #region CenterRatioY

  private double _centerRatioY;

  public double CenterRatioY
  {
    get => _centerRatioY;
    set
    {
      _centerRatioY = value;
      OnPropertyChanged();
    }
  }

  #endregion CenterRatioY

  #region CombinedZoomFactor

  private double _combinedZoomFactor;

  public double CombinedZoomFactor
  {
    get => _combinedZoomFactor;
    set
    {
      _combinedZoomFactor = value;
      OnPropertyChanged();
      CombinedZoomFactorFormatted = FormatZoomFactor();
    }
  }

  #endregion CombinedZoomFactor

  #region CombinedZoomFactorFormatted

  private string _combinedZoomFactorFormatted;

  public string CombinedZoomFactorFormatted
  {
    get => _combinedZoomFactorFormatted;
    set
    {
      if (_combinedZoomFactorFormatted == value)
        return;

      _combinedZoomFactorFormatted = value;
      OnPropertyChanged();
    }
  }

  #endregion CombinedZoomFactorFormatted

  #region JuliaOriginRatioStartX

  private double _juliaOriginRatioStartX;

  public double JuliaOriginRatioStartX
  {
    get => _juliaOriginRatioStartX;
    set
    {
      _juliaOriginRatioStartX = value;
      OnPropertyChanged();
    }
  }

  #endregion JuliaOriginRatioStartX

  #region JuliaOriginRatioStartY

  private double _juliaOriginRatioStartY;

  public double JuliaOriginRatioStartY
  {
    get => _juliaOriginRatioStartY;
    set
    {
      _juliaOriginRatioStartY = value;
      OnPropertyChanged();
    }
  }

  #endregion JuliaOriginRatioStartY

  #region JuliaOriginRatioEndX

  private double _juliaOriginRatioEndX;

  public double JuliaOriginRatioEndX
  {
    get => _juliaOriginRatioEndX;
    set
    {
      _juliaOriginRatioEndX = value;
      OnPropertyChanged();
    }
  }

  #endregion JuliaOriginRatioEndX

  #region JuliaOriginRatioEndY

  private double _juliaOriginRatioEndY;

  public double JuliaOriginRatioEndY
  {
    get => _juliaOriginRatioEndY;
    set
    {
      _juliaOriginRatioEndY = value;
      OnPropertyChanged();
    }
  }

  #endregion JuliaOriginRatioEndY

  #region AnimatedJuliaFrames

  private int _animatedJuliaFrames;

  public int AnimatedJuliaFrames
  {
    get => _animatedJuliaFrames;
    set
    {
      if (_animatedJuliaFrames == value)
        return;

      _animatedJuliaFrames = value;
      OnPropertyChanged();
    }
  }

  #endregion AnimatedJuliaFrames

  #region IsDrawingJuliaOriginLine

  private bool _isDrawingJuliaOriginLine;

  public bool IsDrawingJuliaOriginLine
  {
    get => _isDrawingJuliaOriginLine;
    set
    {
      if (_isDrawingJuliaOriginLine == value)
        return;

      _isDrawingJuliaOriginLine = value;
      OnPropertyChanged();
      DrawJuliaOriginLineCommand.RaiseCanExecuteChanged();
      DrawJuliaOriginLineCancelCommand.RaiseCanExecuteChanged();
      if (!_isDrawingJuliaOriginLine)
      {
        // Drawing complete, so start drawing the animation.
        StartAnimateJulia();
      }
    }
  }

  #endregion IsDrawingJuliaOriginLine

  #region IsBusy

  private bool _isBusy;

  public bool IsBusy
  {
    get => _isBusy;
    set
    {
      if (_isBusy == value)
        return;

      _isBusy = value;
      OnPropertyChanged();
      RaiseCommandNotifications();
    }
  }

  #endregion IsBusy

  #region Percent

  private double _percent;

  public double Percent
  {
    get => _percent;
    set
    {
      if (!(Math.Abs(_percent - value) > .01))
        return;

      _percent = value;
      OnPropertyChanged();
    }
  }

  #endregion Percent

  #region ShowAnimatedJulia

  private bool _showAnimatedJulia;

  public bool ShowAnimatedJulia
  {
    get => _showAnimatedJulia;
    set
    {
      if (_showAnimatedJulia == value)
        return;

      _showAnimatedJulia = value;
      OnPropertyChanged();
    }
  }

  #endregion ShowAnimatedJulia

  #region AnimatedJuliaImage

  private BitmapImage? _animatedJuliaImage;

  public BitmapImage? AnimatedJuliaImage
  {
    get => _animatedJuliaImage;
    set
    {
      if (_animatedJuliaImage == value)
        return;

      _animatedJuliaImage = value;
      OnPropertyChanged();
    }
  }

  #endregion AnimatedJuliaImage
}