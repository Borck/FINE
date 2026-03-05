namespace ExampleCodeGenApp.Views;

using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using ExampleCodeGenApp.ViewModels;
using ReactiveUI;

public partial class CodeSimView : UserControl, IViewFor<CodeSimViewModel> {
  #region ViewModel
  public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
      typeof(CodeSimViewModel), typeof(CodeSimView), new PropertyMetadata(null));

  public CodeSimViewModel ViewModel {
    get => (CodeSimViewModel)GetValue(ViewModelProperty);
    set => SetValue(ViewModelProperty, value);
  }

  object IViewFor.ViewModel {
    get => ViewModel;
    set => ViewModel = (CodeSimViewModel)value;
  }
  #endregion

  public CodeSimView() {
    InitializeComponent();

    this.WhenActivated(d => {
      this.OneWayBind(ViewModel, vm => vm.RunScript, v => v.runButton.Command).DisposeWith(d);
      this.OneWayBind(ViewModel, vm => vm.ClearOutput, v => v.clearButton.Command).DisposeWith(d);
      this.OneWayBind(ViewModel, vm => vm.Output, v => v.outputTextBlock.Text).DisposeWith(d);
    });
  }
}
