namespace ExampleCodeGenApp.Views;

using System.Reactive.Disposables;
using System.Windows;
using ExampleCodeGenApp.ViewModels;
using ReactiveUI;

public partial class CodeGenPendingConnectionView : IViewFor<CodeGenPendingConnectionViewModel> {
  #region ViewModel
  public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
      typeof(CodeGenPendingConnectionViewModel), typeof(CodeGenPendingConnectionView), new PropertyMetadata(null));

  public CodeGenPendingConnectionViewModel ViewModel {
    get => (CodeGenPendingConnectionViewModel)GetValue(ViewModelProperty);
    set => SetValue(ViewModelProperty, value);
  }

  object IViewFor.ViewModel {
    get => ViewModel;
    set => ViewModel = (CodeGenPendingConnectionViewModel)value;
  }
  #endregion

  public CodeGenPendingConnectionView() {
    InitializeComponent();

    this.WhenActivated(d => {
      PendingConnectionView.ViewModel = ViewModel;
      d(Disposable.Create(() => PendingConnectionView.ViewModel = null));
    });
  }
}
