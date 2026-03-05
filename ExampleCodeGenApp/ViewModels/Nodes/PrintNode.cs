namespace ExampleCodeGenApp.ViewModels.Nodes;

using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using ExampleCodeGenApp.Model;
using ExampleCodeGenApp.Model.Compiler;
using ExampleCodeGenApp.Views;
using NodeNetwork.Toolkit.ValueNode;
using ReactiveUI;

public class PrintNode : CodeGenNodeViewModel {
  static PrintNode() {
    Splat.Locator.CurrentMutable.Register(() => new CodeGenNodeView(), typeof(IViewFor<PrintNode>));
  }

  public ValueNodeInputViewModel<ITypedExpression<string>> Text { get; }

  public ValueNodeOutputViewModel<IStatement> Flow { get; }

  public PrintNode() : base(NodeType.Function) {
    Name = "Print";

    Text = new CodeGenInputViewModel<ITypedExpression<string>>(PortType.String) {
      Name = "Text"
    };
    Inputs.Add(Text);

    Flow = new CodeGenOutputViewModel<IStatement>(PortType.Execution) {
      Name = "",
      Value = Text.ValueChanged.Select(stringExpr => new FunctionCall {
        FunctionName = "print",
        Parameters =
          {
                      stringExpr ?? new StringLiteral{Value = ""}
                  }
      })
    };
    Outputs.Add(Flow);
  }
}
