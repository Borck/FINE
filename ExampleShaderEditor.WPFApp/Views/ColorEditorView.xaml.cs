namespace ExampleShaderEditorApp.Views;

using System.Windows;
using ExampleShaderEditorApp.ViewModels.Editors;
using ReactiveUI;

public partial class ColorEditorView : IViewFor<ColorEditorViewModel> {
  #region ViewModel
  public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
      typeof(ColorEditorViewModel), typeof(ColorEditorView), new PropertyMetadata(null));

  public ColorEditorViewModel ViewModel {
    get => (ColorEditorViewModel)GetValue(ViewModelProperty);
    set => SetValue(ViewModelProperty, value);
  }

  object IViewFor.ViewModel {
    get => ViewModel;
    set => ViewModel = (ColorEditorViewModel)value;
  }
  #endregion

  public ColorEditorView() {
    InitializeComponent();

    this.WhenActivated(d => d(
        this.Bind(ViewModel, vm => vm.ColorValue, v => v.colorPicker.SelectedColor)
    ));
  }
}
