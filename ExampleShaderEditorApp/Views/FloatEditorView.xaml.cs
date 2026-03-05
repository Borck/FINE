namespace ExampleShaderEditorApp.Views;

using System.Windows;
using ExampleShaderEditorApp.ViewModels.Editors;
using ReactiveUI;

public partial class FloatEditorView : IViewFor<FloatEditorViewModel> {
  #region ViewModel
  public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
      typeof(FloatEditorViewModel), typeof(FloatEditorView), new PropertyMetadata(null));

  public FloatEditorViewModel ViewModel {
    get => (FloatEditorViewModel)GetValue(ViewModelProperty);
    set => SetValue(ViewModelProperty, value);
  }

  object IViewFor.ViewModel {
    get => ViewModel;
    set => ViewModel = (FloatEditorViewModel)value;
  }
  #endregion

  public FloatEditorView() {
    InitializeComponent();

    this.WhenActivated(d => d(
        this.Bind(ViewModel, vm => vm.FloatValue, v => v.upDown.Value)
    ));
  }
}
