namespace ExampleCodeGenApp.ViewModels;

using NodeNetwork.ViewModels;
using ReactiveUI;

public enum PortType {
  Execution, Integer, String
}

public class CodeGenPortViewModel : PortViewModel {
  static CodeGenPortViewModel() {
    Splat.Locator.CurrentMutable.Register(() => new Views.CodeGenPortView(), typeof(IViewFor<CodeGenPortViewModel>));
  }

  #region PortType
  public PortType PortType {
    get => _portType;
    set => this.RaiseAndSetIfChanged(ref _portType, value);
  }
  private PortType _portType;
  #endregion
}
