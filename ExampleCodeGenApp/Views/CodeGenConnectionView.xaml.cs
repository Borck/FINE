namespace ExampleCodeGenApp.Views;

using System.Reactive.Disposables;
using System.Windows;
using ExampleCodeGenApp.ViewModels;
using ReactiveUI;

public partial class CodeGenConnectionView : IViewFor<CodeGenConnectionViewModel> {
  #region ViewModel
  public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
      typeof(CodeGenConnectionViewModel), typeof(CodeGenConnectionView), new PropertyMetadata(null));

  public CodeGenConnectionViewModel ViewModel {
    get => (CodeGenConnectionViewModel)GetValue(ViewModelProperty);
    set => SetValue(ViewModelProperty, value);
  }

  object IViewFor.ViewModel {
    get => ViewModel;
    set => ViewModel = (CodeGenConnectionViewModel)value;
  }
  #endregion

  public CodeGenConnectionView() {
    InitializeComponent();

    this.WhenActivated(d => {
      ConnectionView.ViewModel = ViewModel;
      d(Disposable.Create(() => ConnectionView.ViewModel = null));
    });
  }
}
