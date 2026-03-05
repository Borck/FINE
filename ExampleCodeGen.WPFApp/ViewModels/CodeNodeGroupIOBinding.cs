namespace ExampleCodeGenApp.ViewModels;

using DynamicData;
using ExampleCodeGenApp.ViewModels.Nodes;
using FINE.Toolkit.Group;
using FINE.Toolkit.ValueNode;
using FINE.ViewModels;

public class CodeNodeGroupIOBinding : ValueNodeGroupIOBinding {
  public CodeNodeGroupIOBinding(NodeViewModel groupNode, NodeViewModel entranceNode, NodeViewModel exitNode) : base(groupNode, entranceNode, exitNode) {
  }

  #region Endpoint Create
  public override ValueNodeOutputViewModel<T> CreateCompatibleOutput<T>(ValueNodeInputViewModel<T> input) => new CodeGenOutputViewModel<T>(((CodeGenPortViewModel)input.Port).PortType) {
    Name = input.Name,
    Editor = new GroupEndpointEditorViewModel<T>(this)
  };

  public override ValueNodeOutputViewModel<IObservableList<T>> CreateCompatibleOutput<T>(ValueListNodeInputViewModel<T> input) => new CodeGenOutputViewModel<IObservableList<T>>(((CodeGenPortViewModel)input.Port).PortType) {
    Editor = new GroupEndpointEditorViewModel<IObservableList<T>>(this)
  };

  public override ValueNodeInputViewModel<T> CreateCompatibleInput<T>(ValueNodeOutputViewModel<T> output) => new CodeGenInputViewModel<T>(((CodeGenPortViewModel)output.Port).PortType) {
    Name = output.Name,
    Editor = new GroupEndpointEditorViewModel<T>(this),
    HideEditorIfConnected = false
  };

  public override ValueListNodeInputViewModel<T> CreateCompatibleInput<T>(ValueNodeOutputViewModel<IObservableList<T>> output) => new CodeGenListInputViewModel<T>(((CodeGenPortViewModel)output.Port).PortType) {
    Name = output.Name,
    Editor = new GroupEndpointEditorViewModel<T>(this),
    HideEditorIfConnected = false
  };
  #endregion
}
