namespace NodeNetwork.Toolkit.BreadcrumbBar;

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using NodeNetwork.Utilities;
using ReactiveUI;
using ReactiveUI.Legacy;

public partial class BreadcrumbBarView : UserControl, IViewFor<BreadcrumbBarViewModel> {
  #region ViewModel
  public static readonly DependencyProperty ViewModelProperty =
      DependencyProperty.Register(nameof(ViewModel), typeof(BreadcrumbBarViewModel), typeof(BreadcrumbBarView), new PropertyMetadata(null));

  public BreadcrumbBarViewModel ViewModel {
    get => (BreadcrumbBarViewModel)GetValue(ViewModelProperty);
    set => SetValue(ViewModelProperty, value);
  }

  object IViewFor.ViewModel {
    get => ViewModel;
    set => ViewModel = (BreadcrumbBarViewModel)value;
  }
  #endregion

  public BreadcrumbBarView() {
    InitializeComponent();

    this.WhenActivated(d => {
      this.BindList(ViewModel, vm => vm.ActivePath, v => v.list.ItemsSource).DisposeWith(d);
      this.WhenAnyValue(v => v.list.SelectedItem)
          .Where(i => i != null)
          .Cast<BreadcrumbViewModel>()
          .Do(_ => list.UnselectAll())
          .InvokeCommand(this, v => v.ViewModel.SelectCrumb).DisposeWith(d);
    });
  }
}
