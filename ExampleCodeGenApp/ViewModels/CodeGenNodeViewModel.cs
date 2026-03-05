namespace ExampleCodeGenApp.ViewModels;

using ExampleCodeGenApp.Views;
using NodeNetwork.ViewModels;
using ReactiveUI;

public enum NodeType {
  EventNode, Function, FlowControl, Literal, Group
}

public class CodeGenNodeViewModel : NodeViewModel {
  static CodeGenNodeViewModel() {
    Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<CodeGenNodeViewModel>));
  }

  public NodeType NodeType { get; }

  public CodeGenNodeViewModel(NodeType type) {
    NodeType = type;
  }
}
