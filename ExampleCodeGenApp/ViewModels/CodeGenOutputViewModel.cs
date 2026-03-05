namespace ExampleCodeGenApp.ViewModels;

using FINE.Toolkit.ValueNode;
using FINE.ViewModels;
using FINE.Views;
using ReactiveUI;

public class CodeGenOutputViewModel<T> : ValueNodeOutputViewModel<T> {
  static CodeGenOutputViewModel() {
    Splat.Locator.CurrentMutable.Register(() => new NodeOutputView(), typeof(IViewFor<CodeGenOutputViewModel<T>>));
  }

  public CodeGenOutputViewModel(PortType type) {
    Port = new CodeGenPortViewModel { PortType = type };

    if (type == PortType.Execution) {
      PortPosition = PortPosition.Left;
    }
  }
}
