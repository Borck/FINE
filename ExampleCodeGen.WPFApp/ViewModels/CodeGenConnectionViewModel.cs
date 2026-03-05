namespace ExampleCodeGenApp.ViewModels;

using ExampleCodeGenApp.Views;
using FINE.ViewModels;
using ReactiveUI;

public class CodeGenConnectionViewModel : ConnectionViewModel {
  static CodeGenConnectionViewModel() {
    Splat.Locator.CurrentMutable.Register(() => new CodeGenConnectionView(), typeof(IViewFor<CodeGenConnectionViewModel>));
  }

  public CodeGenConnectionViewModel(NetworkViewModel parent, NodeInputViewModel input, NodeOutputViewModel output) : base(parent, input, output) {

  }
}
