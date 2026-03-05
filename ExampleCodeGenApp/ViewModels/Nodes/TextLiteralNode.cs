namespace ExampleCodeGenApp.ViewModels.Nodes;

using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using ExampleCodeGenApp.Model;
using ExampleCodeGenApp.Model.Compiler;
using ExampleCodeGenApp.ViewModels.Editors;
using ExampleCodeGenApp.Views;
using NodeNetwork.Toolkit.ValueNode;
using ReactiveUI;

public class TextLiteralNode : CodeGenNodeViewModel {
  static TextLiteralNode() {
    Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<TextLiteralNode>));
  }

  public StringValueEditorViewModel ValueEditor { get; } = new StringValueEditorViewModel();

  public ValueNodeOutputViewModel<ITypedExpression<string>> Output { get; }

  public TextLiteralNode() : base(NodeType.Literal) {
    Name = "Text";

    Output = new CodeGenOutputViewModel<ITypedExpression<string>>(PortType.String) {
      Name = "Value",
      Editor = ValueEditor,
      Value = ValueEditor.ValueChanged.Select(v => new StringLiteral { Value = v })
    };
    Outputs.Add(Output);
  }
}
