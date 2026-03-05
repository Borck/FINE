namespace ExampleCodeGenApp.Views;

using System.Reactive.Disposables;
using System.Windows;
using ExampleCodeGenApp.ViewModels.Nodes;
using ReactiveUI;

public partial class GroupNodeView : IViewFor<GroupNodeViewModel> {
  #region ViewModel
  public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
      typeof(GroupNodeViewModel), typeof(GroupNodeView), new PropertyMetadata(null));

  public GroupNodeViewModel ViewModel {
    get => (GroupNodeViewModel)GetValue(ViewModelProperty);
    set => SetValue(ViewModelProperty, value);
  }

  object IViewFor.ViewModel {
    get => ViewModel;
    set => ViewModel = (GroupNodeViewModel)value;
  }
  #endregion

  public GroupNodeView() {
    InitializeComponent();

    this.WhenActivated(d => {
      NodeView.ViewModel = ViewModel;
      Disposable.Create(() => NodeView.ViewModel = null).DisposeWith(d);

      this.OneWayBind(ViewModel, vm => vm.NodeType, v => v.NodeView.Background, CodeGenNodeView.ConvertNodeTypeToBrush).DisposeWith(d);

    });
  }
}
