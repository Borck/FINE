namespace FINE.ViewModels;

using FINE.Views;
using ReactiveUI;

/// <summary>
/// The viewmodel for the editor component that is displayed next to a node endpoint.
/// </summary>
public class NodeEndpointEditorViewModel : ReactiveObject {
  static NodeEndpointEditorViewModel() {
    NNViewRegistrar.AddRegistration(() => new NodeEndpointEditorView(), typeof(IViewFor<NodeEndpointEditorViewModel>));
  }

  #region Logger
  private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
  #endregion

  #region Parent
  /// <summary>
  /// The endpoint that has this object as its editor.
  /// </summary>
  public Endpoint Parent {
    get => _parent;
    internal set => this.RaiseAndSetIfChanged(ref _parent, value);
  }
  private Endpoint _parent;
  #endregion
}
