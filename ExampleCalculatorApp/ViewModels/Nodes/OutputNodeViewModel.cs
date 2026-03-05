namespace ExampleCalculatorApp.ViewModels.Nodes;

using DynamicData;
using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using ReactiveUI;

public class OutputNodeViewModel : NodeViewModel {
  static OutputNodeViewModel() {
    Splat.Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<OutputNodeViewModel>));
  }

  public ValueNodeInputViewModel<int?> ResultInput { get; }

  public OutputNodeViewModel() {
    Name = "Output";

    CanBeRemovedByUser = false;

    ResultInput = new ValueNodeInputViewModel<int?> {
      Name = "Value",
      Editor = new IntegerValueEditorViewModel()
    };
    Inputs.Add(ResultInput);
  }
}
