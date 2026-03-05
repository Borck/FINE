namespace FINE.Views;

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FINE.ViewModels;
using ReactiveUI;

[TemplateVisualState(Name = ErrorState, GroupName = ErrorVisualStatesGroup)]
[TemplateVisualState(Name = NonErrorState, GroupName = ErrorVisualStatesGroup)]
public class PendingConnectionView : Control, IViewFor<PendingConnectionViewModel> {
  #region ViewModel
  public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
      typeof(PendingConnectionViewModel), typeof(PendingConnectionView), new PropertyMetadata(null));

  public PendingConnectionViewModel ViewModel {
    get => (PendingConnectionViewModel)GetValue(ViewModelProperty);
    set => SetValue(ViewModelProperty, value);
  }

  object IViewFor.ViewModel {
    get => ViewModel;
    set => ViewModel = (PendingConnectionViewModel)value;
  }
  #endregion

  #region States
  #region ErrorStates
  public const string ErrorVisualStatesGroup = "ErrorStates";
  public const string ErrorState = "Error";
  public const string NonErrorState = "NoError";
  #endregion
  #endregion

  #region RegularBrush
  public Brush RegularBrush {
    get => (Brush)GetValue(RegularBrushProperty);
    set => SetValue(RegularBrushProperty, value);
  }
  public static readonly DependencyProperty RegularBrushProperty = DependencyProperty.Register(nameof(RegularBrush), typeof(Brush), typeof(PendingConnectionView));
  #endregion

  #region ErrorBrush
  public Brush ErrorBrush {
    get => (Brush)GetValue(ErrorBrushProperty);
    set => SetValue(ErrorBrushProperty, value);
  }
  public static readonly DependencyProperty ErrorBrushProperty = DependencyProperty.Register(nameof(ErrorBrush), typeof(Brush), typeof(PendingConnectionView));
  #endregion

  #region Geometry
  public Geometry Geometry {
    get => (Geometry)GetValue(GeometryProperty);
    set => SetValue(GeometryProperty, value);
  }
  public static readonly DependencyProperty GeometryProperty = DependencyProperty.Register(nameof(Geometry), typeof(Geometry), typeof(PendingConnectionView));
  #endregion

  public PendingConnectionView() {
    DefaultStyleKey = typeof(PendingConnectionView);

    SetupPathData();
    SetupVisualStateBindings();
  }

  public override void OnApplyTemplate() => VisualStateManager.GoToState(this, NonErrorState, false);

  private void SetupPathData() => this.WhenActivated(d => d(
        this.WhenAnyValue(v => v.ViewModel.LooseEndPoint)
            .Select(_ => {
              if (ViewModel.Input == null) {
                return ConnectionView.BuildSmoothBezier(ViewModel.Output.Port.CenterPoint,
                          ViewModel.Output.PortPosition,
                          ViewModel.LooseEndPoint);
              } else if (ViewModel.Output == null) {
                return ConnectionView.BuildSmoothBezier(ViewModel.LooseEndPoint,
                          ViewModel.Input.Port.CenterPoint,
                          ViewModel.Input.PortPosition);
              } else {
                return ConnectionView.BuildSmoothBezier(ViewModel.Output.Port.CenterPoint,
                          ViewModel.Output.PortPosition,
                          ViewModel.Input.Port.CenterPoint,
                          ViewModel.Input.PortPosition);
              }
            })
            .BindTo(this, v => v.Geometry)
    ));

  private void SetupVisualStateBindings() => this.WhenActivated(d => d(
        this.WhenAnyValue(v => v.ViewModel.Validation.IsValid).Subscribe(isValid => {
          VisualStateManager.GoToState(this, isValid ? NonErrorState : ErrorState, true);
        })
    ));
}
