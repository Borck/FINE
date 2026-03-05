namespace ExampleShaderEditorApp.Views;

using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using ExampleShaderEditorApp.Model;
using ExampleShaderEditorApp.ViewModels.Editors;
using ReactiveUI;

public partial class Vec2EditorView : IViewFor<Vec2EditorViewModel> {
  #region ViewModel
  public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
      typeof(Vec2EditorViewModel), typeof(Vec2EditorView), new PropertyMetadata(null));

  public Vec2EditorViewModel ViewModel {
    get => (Vec2EditorViewModel)GetValue(ViewModelProperty);
    set => SetValue(ViewModelProperty, value);
  }

  object IViewFor.ViewModel {
    get => ViewModel;
    set => ViewModel = (Vec2EditorViewModel)value;
  }
  #endregion

  public Vec2EditorView() {
    InitializeComponent();

    this.WhenActivated(d => {
      this.WhenAnyValue(v => v.xUpDown.Value, v => v.yUpDown.Value)
          .Select(c => new Vec2(c.Item1 ?? 0, c.Item2 ?? 0))
          .BindTo(this, v => v.ViewModel.Vec2Value)
          .DisposeWith(d);
    });
  }
}
