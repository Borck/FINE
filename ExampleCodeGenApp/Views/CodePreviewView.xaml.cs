namespace ExampleCodeGenApp.Views;

using System.Reactive.Disposables;
using System.Windows;
using ExampleCodeGenApp.ViewModels;
using ReactiveUI;

public partial class CodePreviewView : IViewFor<CodePreviewViewModel> {
  #region ViewModel
  public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
      typeof(CodePreviewViewModel), typeof(CodePreviewView), new PropertyMetadata(null));

  public CodePreviewViewModel ViewModel {
    get => (CodePreviewViewModel)GetValue(ViewModelProperty);
    set => SetValue(ViewModelProperty, value);
  }

  object IViewFor.ViewModel {
    get => ViewModel;
    set => ViewModel = (CodePreviewViewModel)value;
  }
  #endregion

  public CodePreviewView() {
    InitializeComponent();

    this.WhenActivated(d => {
      this.OneWayBind(ViewModel, vm => vm.CompiledCode, v => v.codeTextBlock.Text).DisposeWith(d);
      this.OneWayBind(ViewModel, vm => vm.CompilerError, v => v.errorTextBlock.Text).DisposeWith(d);
    });
  }
}
