namespace ExampleCodeGenApp.ViewModels;

using ExampleCodeGenApp.Views;
using FINE.ViewModels;
using ReactiveUI;

public class CodeGenPendingConnectionViewModel : PendingConnectionViewModel {
  static CodeGenPendingConnectionViewModel() {
    Splat.Locator.CurrentMutable.Register(() => new CodeGenPendingConnectionView(), typeof(IViewFor<CodeGenPendingConnectionViewModel>));
  }

  public CodeGenPendingConnectionViewModel(NetworkViewModel parent) : base(parent) {

  }
}
