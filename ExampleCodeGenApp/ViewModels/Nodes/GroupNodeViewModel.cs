namespace ExampleCodeGenApp.ViewModels.Nodes;

using System;
using ExampleCodeGenApp.Views;
using NodeNetwork.Toolkit.Group.AddEndpointDropPanel;
using NodeNetwork.ViewModels;
using ReactiveUI;

public class GroupNodeViewModel : CodeGenNodeViewModel {
  static GroupNodeViewModel() {
    Splat.Locator.CurrentMutable.Register(() => new GroupNodeView(), typeof(IViewFor<GroupNodeViewModel>));
  }

  public NetworkViewModel Subnet { get; }

  #region IOBinding
  public CodeNodeGroupIOBinding IOBinding {
    get => _ioBinding;
    set {
      if (_ioBinding != null) {
        throw new InvalidOperationException("IOBinding is already set.");
      }
      _ioBinding = value;
      AddEndpointDropPanelVM = new AddEndpointDropPanelViewModel {
        NodeGroupIOBinding = IOBinding
      };
    }
  }
  private CodeNodeGroupIOBinding _ioBinding;
  #endregion

  public AddEndpointDropPanelViewModel AddEndpointDropPanelVM { get; private set; }

  public GroupNodeViewModel(NetworkViewModel subnet) : base(NodeType.Group) {
    Name = "Group";
    Subnet = subnet;
  }
}
