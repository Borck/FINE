namespace ExampleCodeGenApp.ViewModels.Editors;

using ExampleCodeGenApp.Views.Editors;
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
