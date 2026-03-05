namespace ExampleCodeGenApp.ViewModels.Nodes;

using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using ExampleCodeGenApp.Model;
using ExampleCodeGenApp.Model.Compiler;
using ExampleCodeGenApp.ViewModels.Editors;
using ExampleCodeGenApp.Views;
using FINE.Toolkit.ValueNode;
using ReactiveUI;

public class IntLiteralNode : CodeGenNodeViewModel {
  static IntLiteralNode() {
    Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<IntLiteralNode>));
  }

  public IntegerValueEditorViewModel ValueEditor { get; } = new IntegerValueEditorViewModel();

  public ValueNodeOutputViewModel<ITypedExpression<int>> Output { get; }

  public IntLiteralNode() : base(NodeType.Literal) {
    Name = "Integer";

    Output = new CodeGenOutputViewModel<ITypedExpression<int>>(PortType.Integer) {
      Editor = ValueEditor,
      Value = ValueEditor.ValueChanged.Select(v => new IntLiteral { Value = v ?? 0 })
    };
    Outputs.Add(Output);
  }
}
