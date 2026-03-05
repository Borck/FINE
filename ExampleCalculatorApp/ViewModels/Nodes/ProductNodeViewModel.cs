namespace ExampleCalculatorApp.ViewModels.Nodes;

using System.Reactive.Linq;
using DynamicData;
using FINE.Toolkit.ValueNode;
using FINE.ViewModels;
using FINE.Views;
using ReactiveUI;

public class ProductNodeViewModel : NodeViewModel {
  static ProductNodeViewModel() {
    Splat.Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<ProductNodeViewModel>));
  }

  public ValueNodeInputViewModel<int?> Input1 { get; }
  public ValueNodeInputViewModel<int?> Input2 { get; }
  public ValueNodeOutputViewModel<int?> Output { get; }

  public ProductNodeViewModel() {
    Name = "Product";

    Input1 = new ValueNodeInputViewModel<int?> {
      Name = "A",
      Editor = new IntegerValueEditorViewModel()
    };
    Inputs.Add(Input1);

    Input2 = new ValueNodeInputViewModel<int?> {
      Name = "B",
      Editor = new IntegerValueEditorViewModel()
    };
    Inputs.Add(Input2);

    var product = this.WhenAnyValue(vm => vm.Input1.Value, vm => vm.Input2.Value)
        .Select(_ => Input1.Value != null && Input2.Value != null ? Input1.Value * Input2.Value : null);

    Output = new ValueNodeOutputViewModel<int?> {
      Name = "A * B",
      Value = product
    };
    Outputs.Add(Output);
  }
}
