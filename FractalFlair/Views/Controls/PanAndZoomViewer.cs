using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WeakEventManager = Common.WeakEventManager;

namespace FractalFlair.Views.Controls;

public class PanAndZoomViewer : ContentControl
{
  public delegate void PanEventHandler(object sender, PanEventArgs e);

  public delegate void ZoomEventHandler(object sender, ZoomEventArgs e);

  private bool _hasChanged;
  private Point _screenStartPoint = new(0, 0);
  private FrameworkElement? _childImage;
  private Grid? _childGrid;
  private Point _startingPoint;
  private Point _startOffset;
  private readonly TranslateTransform _panTransform;
  private readonly TransformGroup _transformGroup;
  private readonly ScaleTransform _zoomTransform;
  private readonly WeakEventManager _eventManager = new();
  private const double _defaultZoomFactor = 1.4d;

  public PanAndZoomViewer()
  {
    _panTransform = new TranslateTransform();
    _zoomTransform = new ScaleTransform();
    _transformGroup = new TransformGroup();
    _transformGroup.Children.Add(_zoomTransform);
    _transformGroup.Children.Add(_panTransform);
    CurrentZoomLevel = 1d;
  }

  public static readonly DependencyProperty IsDrawingProperty =
    DependencyProperty.Register(
      nameof(IsDrawing),
      typeof(bool),
      typeof(PanAndZoomViewer));

  public static readonly DependencyProperty LineStartXProperty =
    DependencyProperty.Register(
      nameof(LineStartX),
      typeof(double),
      typeof(PanAndZoomViewer));

  public static readonly DependencyProperty LineStartYProperty =
    DependencyProperty.Register(
      nameof(LineStartY),
      typeof(double),
      typeof(PanAndZoomViewer));

  public static readonly DependencyProperty LineEndXProperty =
    DependencyProperty.Register(
      nameof(LineEndX),
      typeof(double),
      typeof(PanAndZoomViewer));

  public static readonly DependencyProperty LineEndYProperty =
    DependencyProperty.Register(
      nameof(LineEndY),
      typeof(double),
      typeof(PanAndZoomViewer));

  public static readonly DependencyProperty CenterXProperty =
    DependencyProperty.Register(
      nameof(CenterX),
      typeof(double),
      typeof(PanAndZoomViewer));

  public static readonly DependencyProperty CenterYProperty =
    DependencyProperty.Register(
      nameof(CenterY),
      typeof(double),
      typeof(PanAndZoomViewer));

  public static readonly DependencyProperty CurrentZoomLevelProperty =
    DependencyProperty.Register(
      nameof(CurrentZoomLevel),
      typeof(double),
      typeof(PanAndZoomViewer),
      new UIPropertyMetadata(0.01d));

  public static readonly DependencyProperty ZoomedHeightProperty =
    DependencyProperty.Register(
      nameof(ZoomedHeight),
      typeof(double),
      typeof(PanAndZoomViewer),
      new UIPropertyMetadata(0.01d));

  public static readonly DependencyProperty ZoomedWidthProperty =
    DependencyProperty.Register(
      nameof(ZoomedWidth),
      typeof(double),
      typeof(PanAndZoomViewer),
      new UIPropertyMetadata(0.01d));

  public static readonly DependencyProperty CenterRatioXProperty =
    DependencyProperty.Register(
      nameof(CenterRatioX),
      typeof(double),
      typeof(PanAndZoomViewer),
      new UIPropertyMetadata(0.01d));

  public static readonly DependencyProperty CenterRatioYProperty =
    DependencyProperty.Register(
      nameof(CenterRatioY),
      typeof(double),
      typeof(PanAndZoomViewer),
      new UIPropertyMetadata(0.01d));

  public static readonly DependencyProperty ImageSourceProperty =
    DependencyProperty.Register(
      nameof(ImageSource),
      typeof(ImageSource),
      typeof(PanAndZoomViewer),
      new UIPropertyMetadata(null));

  public event PanEventHandler? PanBeginning
  {
    add => _eventManager.AddEventHandler(value);
    remove => _eventManager.RemoveEventHandler(value);
  }

  public event PanEventHandler? PanCompleted
  {
    add => _eventManager.AddEventHandler(value);
    remove => _eventManager.RemoveEventHandler(value);
  }

  public event ZoomEventHandler? ZoomComplete
  {
    add => _eventManager.AddEventHandler(value);
    remove => _eventManager.RemoveEventHandler(value);
  }

  public bool IsDrawing
  {
    get => (bool)GetValue(IsDrawingProperty);
    set => SetValue(IsDrawingProperty, value);
  }

  public double CurrentZoomLevel
  {
    get => (double)GetValue(CurrentZoomLevelProperty);
    set => SetValue(CurrentZoomLevelProperty, value);
  }

  public double LineStartX
  {
    get => (double)GetValue(LineStartXProperty);
    set => SetValue(LineStartXProperty, value);
  }

  public double LineStartY
  {
    get => (double)GetValue(LineStartYProperty);
    set => SetValue(LineStartYProperty, value);
  }

  public double LineEndX
  {
    get => (double)GetValue(LineEndXProperty);
    set => SetValue(LineEndXProperty, value);
  }

  public double LineEndY
  {
    get => (double)GetValue(LineEndYProperty);
    set => SetValue(LineEndYProperty, value);
  }

  public double CenterX
  {
    get => (double)GetValue(CenterXProperty);
    set => SetValue(CenterXProperty, value);
  }

  public double CenterY
  {
    get => (double)GetValue(CenterYProperty);
    set => SetValue(CenterYProperty, value);
  }

  public double ZoomedWidth
  {
    get => (double)GetValue(ZoomedWidthProperty);
    set => SetValue(ZoomedWidthProperty, value);
  }

  public double ZoomedHeight
  {
    get => (double)GetValue(ZoomedHeightProperty);
    set => SetValue(ZoomedHeightProperty, value);
  }

  public double CenterRatioX
  {
    get => (double)GetValue(CenterRatioXProperty);
    set => SetValue(CenterRatioXProperty, value);
  }

  public double CenterRatioY
  {
    get => (double)GetValue(CenterRatioYProperty);
    set => SetValue(CenterRatioYProperty, value);
  }

  public ImageSource ImageSource
  {
    get => (ImageSource)GetValue(ImageSourceProperty);
    set => SetValue(ImageSourceProperty, value);
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    Setup();
  }

  public void ForceReset()
  {
    _hasChanged = true;
    Reset();
  }

  private void Setup()
  {
    var presenter = VisualTreeHelper.GetChild(this, 0) as ContentPresenter;
    _childGrid = presenter?.Content as Grid;
    if (_childGrid == null)
      throw new Exception($"{nameof(PanAndZoomViewer)} must contain one {typeof(Grid).FullName} for drawing lines.");

    _childImage = _childGrid.Children.OfType<Image>().FirstOrDefault();
    if (_childImage == null)
      throw new Exception($"{nameof(PanAndZoomViewer)}'s {nameof(Grid)} must contain one {typeof(Image).FullName} for displaying the fractal.");

    _childImage.RenderTransform = _transformGroup;

    Focusable = true;
    MouseMove += PanAndZoomViewer_MouseMove;
    MouseLeftButtonDown += PanAndZoomViewer_MouseLeftButtonDown;
    MouseLeftButtonUp += PanAndZoomViewer_MouseLeftButtonUp;
    MouseWheel += PanAndZoomViewer_MouseWheel;
  }

  private void PanAndZoomViewer_MouseWheel(object sender, MouseWheelEventArgs e)
  {
    if (e.Delta == 0)
      return;

    if (_transformGroup.Inverse == null)
      return;

    if (IsDrawing)
      return;

    _hasChanged = true;

    double zoomFactor;
    var zoom = e.Delta / 120d;

    if (zoom >= 0)
      zoomFactor = _defaultZoomFactor * zoom;
    else
      zoomFactor = 1d / (_defaultZoomFactor * -zoom);

    var physicalPoint = new Point(ActualWidth / 2d, ActualHeight / 2d);
    DoZoom(zoomFactor, _transformGroup.Inverse.Transform(physicalPoint), physicalPoint);
  }

  private void PanAndZoomViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    // Save starting point, used later when determining how much to scroll.
    _screenStartPoint = e.GetPosition(this);
    if (IsDrawing)
    {
      if (_childGrid == null)
        return;

      LineStartX = _screenStartPoint.X / ZoomedWidth;
      LineStartY = _screenStartPoint.Y / ZoomedHeight;
      LineEndX = _screenStartPoint.X / ZoomedWidth;
      LineEndY = _screenStartPoint.Y / ZoomedHeight;

      var gridStartPoint = e.GetPosition(_childGrid);
      var lineBackground = new Line
      {
        Stroke = Brushes.White,
        StrokeThickness = 3d,
        X1 = gridStartPoint.X,
        Y1 = gridStartPoint.Y,
        X2 = gridStartPoint.X,
        Y2 = gridStartPoint.Y
      };

      var lineForeground = new Line
      {
        Stroke = Brushes.Black,
        StrokeThickness = 1d,
        X1 = gridStartPoint.X,
        Y1 = gridStartPoint.Y,
        X2 = gridStartPoint.X,
        Y2 = gridStartPoint.Y
      };

      _childGrid.Children.Add(lineBackground);
      _childGrid.Children.Add(lineForeground);
      // Shouldn't be necessary, but here's how to set ZIndex
      Panel.SetZIndex(lineBackground, 1);
      Panel.SetZIndex(lineForeground, 5);
      Cursor = Cursors.Pen;
    }
    else
    {
      _startOffset = new Point(_panTransform.X, _panTransform.Y);
      Cursor = Cursors.ScrollAll;
    }

    CaptureMouse();
  }

  private void PanAndZoomViewer_MouseMove(object sender, MouseEventArgs e)
  {
    if (!IsMouseCaptured)
      return;

    if (e.LeftButton != MouseButtonState.Pressed)
      return;

    _hasChanged = true;

    // if the mouse is captured then move the content by changing the translate transform.  
    // use the Pan Animation to animate to the new location based on the delta between the 
    // starting point of the mouse and the current point.
    var physicalPoint = e.GetPosition(this);
    if (IsDrawing)
    {
      var lines = _childGrid?.Children.OfType<Line>().ToArray() ?? Array.Empty<Line>();
      if (!lines.Any())
        return;

      LineEndX = physicalPoint.X / ZoomedWidth;
      LineEndY = physicalPoint.Y / ZoomedHeight;

      var endPoint = e.GetPosition(_childGrid);
      foreach (var line in lines)
      {
        line.X2 = endPoint.X;
        line.Y2 = endPoint.Y;
      }
    }
    else
    {
      var pe = new PanEventArgs(physicalPoint, false);
      _eventManager.RaiseEvent(this, pe, nameof(PanBeginning));
      if (pe.Cancel)
        return;

      var panX = physicalPoint.X - _screenStartPoint.X + _startOffset.X;
      var panY = physicalPoint.Y - _screenStartPoint.Y + _startOffset.Y;
      CenterX = ActualWidth / 2d - panX;
      CenterY = ActualHeight / 2d - panY;
      CenterRatioX = CenterX / ZoomedWidth;
      CenterRatioY = CenterY / ZoomedHeight;

      _panTransform.BeginAnimation(TranslateTransform.XProperty, CreatePanAnimation(panX), HandoffBehavior.Compose);
      _panTransform.BeginAnimation(TranslateTransform.YProperty, CreatePanAnimation(panY), HandoffBehavior.Compose);
    }
  }

  private void PanAndZoomViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
  {
    if (!IsMouseCaptured)
      return;

    _hasChanged = true;

    // we're done.  reset the cursor and release the mouse pointer
    Cursor = Cursors.Arrow;
    ReleaseMouseCapture();
    if (IsDrawing)
      IsDrawing = false;
    else
    {
      var physicalPoint = e.GetPosition(this);
      var newPoint = new Point
      {
        X = physicalPoint.X - _startingPoint.X,
        Y = physicalPoint.Y - _startingPoint.Y
      };

      var zoomPoint = new Point { X = CenterX, Y = CenterY };
      var pe = new PanEventArgs(zoomPoint, false)
      {
        CurrentZoomLevel = CurrentZoomLevel,
        NoZoomPoint = newPoint
      };

      _eventManager.RaiseEvent(this, pe, nameof(PanCompleted));
    }
  }

  /// <summary>Zoom into or out of the content.</summary>
  /// <param name="deltaZoom">Factor to multiply the zoom level by. </param>
  /// <param name="mousePosition">Logical mouse position relative to the original content.</param>
  /// <param name="physicalPosition">Actual mouse position on the screen (relative to the parent window)</param>
  private void DoZoom(double deltaZoom, Point mousePosition, Point physicalPosition)
  {
    //var currentZoom = _zoomTransform.ScaleX;
    //currentZoom = currentZoom * deltaZoom;
    CurrentZoomLevel = _zoomTransform.ScaleX * deltaZoom;
    ZoomedHeight = ActualHeight * CurrentZoomLevel;
    ZoomedWidth = ActualWidth * CurrentZoomLevel;

    var panX = -(mousePosition.X * CurrentZoomLevel - physicalPosition.X);
    var panY = -(mousePosition.Y * CurrentZoomLevel - physicalPosition.Y);
    CenterX = ActualWidth / 2d - panX;
    CenterY = ActualHeight / 2d - panY;
    CenterRatioX = CenterX / ZoomedWidth;
    CenterRatioY = CenterY / ZoomedHeight;
    var ze = new ZoomEventArgs(CurrentZoomLevel);
    _eventManager.RaiseEvent(this, ze, nameof(ZoomComplete));

    _panTransform.BeginAnimation(TranslateTransform.XProperty, CreateZoomAnimation(panX), HandoffBehavior.Compose);
    _panTransform.BeginAnimation(TranslateTransform.YProperty, CreateZoomAnimation(panY), HandoffBehavior.Compose);
    _zoomTransform.BeginAnimation(ScaleTransform.ScaleXProperty, CreateZoomAnimation(CurrentZoomLevel), HandoffBehavior.Compose);
    _zoomTransform.BeginAnimation(ScaleTransform.ScaleYProperty, CreateZoomAnimation(CurrentZoomLevel), HandoffBehavior.Compose);
  }

  /// <summary>Reset to default zoom level and centered content.</summary>
  private void Reset()
  {
    if (!_hasChanged)
      return;

    if (_childGrid != null)
    {
      _childGrid.Children.Clear();
      if (_childImage != null)
        _childGrid.Children.Add(_childImage);
    }

    CurrentZoomLevel = 1d;
    CenterX = ActualWidth / 2d;
    CenterY = ActualHeight / 2d;
    ZoomedHeight = ActualHeight;
    ZoomedWidth = ActualWidth;
    _panTransform.BeginAnimation(TranslateTransform.XProperty, CreateZoomAnimation(0d, 1d), HandoffBehavior.Compose);
    _panTransform.BeginAnimation(TranslateTransform.YProperty, CreateZoomAnimation(0d, 1d), HandoffBehavior.Compose);
    _zoomTransform.BeginAnimation(ScaleTransform.ScaleXProperty, CreateZoomAnimation(CurrentZoomLevel, 1d), HandoffBehavior.Compose);
    _zoomTransform.BeginAnimation(ScaleTransform.ScaleYProperty, CreateZoomAnimation(CurrentZoomLevel, 1d), HandoffBehavior.Compose);
    _hasChanged = false;
  }

  /// <summary>Helper to create the panning animation for x,y coordinates.</summary>
  /// <param name="toValue">New value of the coordinate.</param>
  /// <returns>Double animation</returns>
  private static DoubleAnimation CreatePanAnimation(double toValue)
  {
    return CreateZoomAnimation(toValue, 300d);
  }

  private static DoubleAnimation CreateZoomAnimation(double toValue, double milliseconds = 500d)
  {
    var da = new DoubleAnimation(toValue, new Duration(TimeSpan.FromMilliseconds(milliseconds)))
    {
      AccelerationRatio = 0.1d,
      DecelerationRatio = 0.9d,
      FillBehavior = FillBehavior.HoldEnd
    };

    da.Freeze();
    return da;
  }
}