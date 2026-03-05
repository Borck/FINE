namespace ExampleCodeGenApp.ViewModels;

using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using ReactiveUI;

public class CodeGenInputViewModel<T> : ValueNodeInputViewModel<T> {
  static CodeGenInputViewModel() {
    Splat.Locator.CurrentMutable.Register(() => new NodeInputView(), typeof(IViewFor<CodeGenInputViewModel<T>>));
  }

  public CodeGenInputViewModel(PortType type) {
    Port = new CodeGenPortViewModel { PortType = type };

    if (type == PortType.Execution) {
      PortPosition = PortPosition.Right;
    }
  }
}
