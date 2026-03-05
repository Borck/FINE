namespace ExampleCodeGenApp.ViewModels.Nodes;

using DynamicData;
using ExampleCodeGenApp.Model.Compiler;
using ExampleCodeGenApp.Views;
using FINE.Toolkit.ValueNode;
using ReactiveUI;

public class ButtonEventNode : CodeGenNodeViewModel {
  static ButtonEventNode() {
    Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<ButtonEventNode>));
  }

  public ValueListNodeInputViewModel<IStatement> OnClickFlow { get; }

  public ButtonEventNode() : base(NodeType.EventNode) {
    Name = "Button Events";

    OnClickFlow = new CodeGenListInputViewModel<IStatement>(PortType.Execution) {
      Name = "On Click"
    };

    Inputs.Add(OnClickFlow);
  }
}
