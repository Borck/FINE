namespace NodeNetwork.ViewModels;

using ReactiveUI;

public class EndpointGroup : ReactiveObject {
  public EndpointGroup Parent { get; }

  #region Name
  private string _name = "";
  public string Name {
    get => _name;
    set => this.RaiseAndSetIfChanged(ref _name, value);
  }
  #endregion

  public EndpointGroup(EndpointGroup parent = null) {
    Parent = parent;
  }

  public EndpointGroup(string name, EndpointGroup parent = null) {
    Parent = parent;
    Name = name;
  }
}
