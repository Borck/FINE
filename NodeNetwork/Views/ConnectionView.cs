namespace NodeNetwork.Views;

using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NodeNetwork.ViewModels;
using ReactiveUI;

[TemplateVisualState(Name = HighlightedState, GroupName = HighlightVisualStatesGroup)]
[TemplateVisualState(Name = NonHighlightedState, GroupName = HighlightVisualStatesGroup)]
[TemplateVisualState(Name = ErrorState, GroupName = ErrorVisualStatesGroup)]
[TemplateVisualState(Name = NonErrorState, GroupName = ErrorVisualStatesGroup)]
[TemplateVisualState(Name = MarkedForDeleteState, GroupName = MarkedForDeleteVisualStatesGroup)]
[TemplateVisualState(Name = NotMarkedForDeleteState, GroupName = MarkedForDeleteVisualStatesGroup)]
public class ConnectionView : Control, IViewFor<ConnectionViewModel> {
  #region ViewModel
  public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
      typeof(ConnectionViewModel), typeof(ConnectionView), new PropertyMetadata(null));

  public ConnectionViewModel ViewModel {
    get => (ConnectionViewModel)GetValue(ViewModelProperty);
    set => SetValue(ViewModelProperty, value);
  }

  object IViewFor.ViewModel {
    get => ViewModel;
    set => ViewModel = (ConnectionViewModel)value;
  }
  #endregion

  #region States
  #region HighlightStates
  public const string HighlightVisualStatesGroup = "HighlightStates";
  public const string HighlightedState = "Highlighted";
  public const string NonHighlightedState = "NonHighlighted";
  #endregion

  #region ErrorStates
  public const string ErrorVisualStatesGroup = "ErrorStates";
  public const string ErrorState = "Error";
  public const string NonErrorState = "NoError";
  #endregion

  #region ErrorStates
  public const string MarkedForDeleteVisualStatesGroup = "MarkedForDeleteStates";
  public const string MarkedForDeleteState = "Marked";
  public const string NotMarkedForDeleteState = "NotMarked";
  #endregion
  #endregion

  #region RegularBrush
  public Brush RegularBrush {
    get => (Brush)GetValue(RegularBrushProperty);
    set => SetValue(RegularBrushProperty, value);
  }
  public static readonly DependencyProperty RegularBrushProperty = DependencyProperty.Register(nameof(RegularBrush), typeof(Brush), typeof(ConnectionView), new PropertyMetadata());
  #endregion

  #region ErrorBrush
  public Brush ErrorBrush {
    get => (Brush)GetValue(ErrorBrushProperty);
    set => SetValue(ErrorBrushProperty, value);
  }
  public static readonly DependencyProperty ErrorBrushProperty = DependencyProperty.Register(nameof(ErrorBrush), typeof(Brush), typeof(ConnectionView), new PropertyMetadata());
  #endregion

  #region HighlightBrush
  public Brush HighlightBrush {
    get => (Brush)GetValue(HighlightBrushProperty);
    set => SetValue(HighlightBrushProperty, value);
  }
  public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(nameof(HighlightBrush), typeof(Brush), typeof(ConnectionView), new PropertyMetadata());
  #endregion

  #region MarkedForDeleteBrush
  public Brush MarkedForDeleteBrush {
    get => (Brush)GetValue(MarkedForDeleteBrushProperty);
    set => SetValue(MarkedForDeleteBrushProperty, value);
  }
  public static readonly DependencyProperty MarkedForDeleteBrushProperty =
      DependencyProperty.Register(nameof(MarkedForDeleteBrush), typeof(Brush), typeof(ConnectionView), new PropertyMetadata());
  #endregion

  #region Geometry
  public Geometry Geometry {
    get => (Geometry)GetValue(GeometryProperty);
    private set => SetValue(GeometryProperty, value);
  }
  public static readonly DependencyProperty GeometryProperty = DependencyProperty.Register(nameof(Geometry), typeof(Geometry), typeof(ConnectionView));
  #endregion

  public ConnectionView() {
    DefaultStyleKey = typeof(ConnectionView);

    SetupPathData();
    SetupBrushesBinding();
  }

  public override void OnApplyTemplate() {
    VisualStateManager.GoToState(this, NonHighlightedState, false);
    VisualStateManager.GoToState(this, NonErrorState, false);
    VisualStateManager.GoToState(this, NotMarkedForDeleteState, false);
  }

  private void SetupPathData() => this.WhenActivated(d => d(
        this.WhenAny(
            v => v.ViewModel.Input.Port.CenterPoint,
            v => v.ViewModel.Input.PortPosition,
            v => v.ViewModel.Output.Port.CenterPoint,
            v => v.ViewModel.Output.PortPosition,
            (a, b, c, e) => (a, b, c, e))
            .Select(_
                => BuildSmoothBezier(
                    ViewModel.Input.Port.CenterPoint,
                    ViewModel.Input.PortPosition,
                    ViewModel.Output.Port.CenterPoint,
                    ViewModel.Output.PortPosition))
            .BindTo(this, v => v.Geometry)
    ));

  private void SetupBrushesBinding() => this.WhenActivated(d => {
    this.WhenAnyValue(v => v.ViewModel.IsHighlighted).Subscribe(isHighlighted => {
      VisualStateManager.GoToState(this, isHighlighted ? HighlightedState : NonHighlightedState, true);
    }).DisposeWith(d);
    this.WhenAnyValue(v => v.ViewModel.IsInErrorState).Subscribe(isInErrorState => {
      VisualStateManager.GoToState(this, isInErrorState ? ErrorState : NonErrorState, true);
    }).DisposeWith(d);
    this.WhenAnyValue(v => v.ViewModel.IsMarkedForDelete).Subscribe(isMarkedForDelete => {
      VisualStateManager.GoToState(this, isMarkedForDelete ? MarkedForDeleteState : NotMarkedForDeleteState, true);
    }).DisposeWith(d);
  });
  public static PathGeometry BuildSmoothBezier(Point startPoint, PortPosition startPosition, Point endPoint, PortPosition endPosition) {
    var startGradient = ToGradient(startPosition);
    var endGradient = ToGradient(endPosition);

    return BuildSmoothBezier(startPoint, startGradient, endPoint, endGradient);
  }

  public static PathGeometry BuildSmoothBezier(Point startPoint, PortPosition startPosition, Point endPoint) {
    var startGradient = ToGradient(startPosition);
    var endGradient = -startGradient;

    return BuildSmoothBezier(startPoint, startGradient, endPoint, endGradient);
  }

  public static PathGeometry BuildSmoothBezier(Point startPoint, Point endPoint, PortPosition endPosition) {
    var endGradient = ToGradient(endPosition);
    var startGradient = -endGradient;

    return BuildSmoothBezier(startPoint, startGradient, endPoint, endGradient);
  }

  private static Vector ToGradient(PortPosition portPosition) {
    switch (portPosition) {
      case PortPosition.Left:
        return new Vector(-1, 0);
      case PortPosition.Right:
        return new Vector(1, 0);
      default:
        throw new NotImplementedException();
    }
  }

  private const double MinGradient = 10;
  private const double WidthScaling = 5;

  private static PathGeometry BuildSmoothBezier(Point startPoint, Vector startGradient, Point endPoint, Vector endGradient) {
    var width = endPoint.X - startPoint.X;

    var gradientScale = Math.Sqrt(Math.Abs(width) * WidthScaling + MinGradient * MinGradient);

    var startGradientPoint = startPoint + startGradient * gradientScale;
    var endGradientPoint = endPoint + endGradient * gradientScale;

    var midPoint = new Point((startGradientPoint.X + endGradientPoint.X) / 2d, (startPoint.Y + endPoint.Y) / 2d);

    var pathFigure = new PathFigure {
      StartPoint = startPoint,
      IsClosed = false,
      Segments =
        {
                  new QuadraticBezierSegment(startGradientPoint, midPoint, true),
                  new QuadraticBezierSegment(endGradientPoint, endPoint, true)
              }
    };

    var geom = new PathGeometry();
    geom.Figures.Add(pathFigure);

    return geom;
  }
}
