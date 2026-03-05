namespace ExampleCalculatorApp.ViewModels.Nodes;

using DynamicData;
using FINE.Toolkit.ValueNode;
using FINE.ViewModels;
using FINE.Views;
using ReactiveUI;

public class ConstantNodeViewModel : NodeViewModel {
  static ConstantNodeViewModel() {
    Splat.Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<ConstantNodeViewModel>));
  }

  public IntegerValueEditorViewModel ValueEditor { get; } = new IntegerValueEditorViewModel();

  public ValueNodeOutputViewModel<int?> Output { get; }

  public ConstantNodeViewModel() {
    Name = "Constant";

    Output = new ValueNodeOutputViewModel<int?> {
      Name = "Value",
      Editor = ValueEditor,
      Value = this.WhenAnyValue(vm => vm.ValueEditor.Value)
    };
    Outputs.Add(Output);
  }
}
