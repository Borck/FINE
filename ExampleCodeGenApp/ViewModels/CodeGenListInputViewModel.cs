namespace ExampleCodeGenApp.ViewModels;

using FINE.Toolkit.ValueNode;
using FINE.ViewModels;
using FINE.Views;
using ReactiveUI;

public class CodeGenListInputViewModel<T> : ValueListNodeInputViewModel<T> {
  static CodeGenListInputViewModel() {
    Splat.Locator.CurrentMutable.Register(() => new NodeInputView(), typeof(IViewFor<CodeGenListInputViewModel<T>>));
  }

  public CodeGenListInputViewModel(PortType type) {
    Port = new CodeGenPortViewModel { PortType = type };

    if (type == PortType.Execution) {
      PortPosition = PortPosition.Right;
    }
  }
}
