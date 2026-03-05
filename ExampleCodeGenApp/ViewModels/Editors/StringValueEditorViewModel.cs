namespace ExampleCodeGenApp.ViewModels.Editors;

using ExampleCodeGenApp.Views.Editors;
using FINE.Toolkit.ValueNode;
using ReactiveUI;

public class StringValueEditorViewModel : ValueEditorViewModel<string> {
  static StringValueEditorViewModel() {
    Splat.Locator.CurrentMutable.Register(() => new StringValueEditorView(), typeof(IViewFor<StringValueEditorViewModel>));
  }

  public StringValueEditorViewModel() {
    Value = "";
  }
}
