namespace ExampleCodeGenApp.Views;

using System.Reactive.Disposables;
using System.Windows;
using ExampleCodeGenApp.ViewModels.Nodes;
using ReactiveUI;

public partial class GroupSubnetIONodeView : IViewFor<GroupSubnetIONodeViewModel> {
  #region ViewModel
  public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
      typeof(GroupSubnetIONodeViewModel), typeof(GroupSubnetIONodeView), new PropertyMetadata(null));

  public GroupSubnetIONodeViewModel ViewModel {
    get => (GroupSubnetIONodeViewModel)GetValue(ViewModelProperty);
    set => SetValue(ViewModelProperty, value);
  }

  object IViewFor.ViewModel {
    get => ViewModel;
    set => ViewModel = (GroupSubnetIONodeViewModel)value;
  }
  #endregion

  public GroupSubnetIONodeView() {
    InitializeComponent();

    this.WhenActivated(d => {
      NodeView.ViewModel = ViewModel;
      Disposable.Create(() => NodeView.ViewModel = null).DisposeWith(d);

      this.OneWayBind(ViewModel, vm => vm.NodeType, v => v.NodeView.Background, CodeGenNodeView.ConvertNodeTypeToBrush).DisposeWith(d);

    });
  }
}
