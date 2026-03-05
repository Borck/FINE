namespace ExampleCodeGenApp.ViewModels;

using FINE.Toolkit.ValueNode;
using FINE.ViewModels;
using FINE.Views;
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
