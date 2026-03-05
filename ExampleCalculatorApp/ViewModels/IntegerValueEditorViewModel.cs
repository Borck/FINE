namespace ExampleCalculatorApp.ViewModels;

using ExampleCalculatorApp.Views;
using NodeNetwork.Toolkit.ValueNode;
using ReactiveUI;

public class IntegerValueEditorViewModel : ValueEditorViewModel<int?> {
  static IntegerValueEditorViewModel() {
    Splat.Locator.CurrentMutable.Register(() => new IntegerValueEditorView(), typeof(IViewFor<IntegerValueEditorViewModel>));
  }

  public IntegerValueEditorViewModel() {
    Value = 0;
  }
}
