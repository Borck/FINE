namespace NodeNetwork.Toolkit.Blazor;

using NodeNetwork.Blazor.Models;

/// <summary>
/// Used by node pickers and factories that need a sample node plus a creation delegate.
/// </summary>
public sealed class NodeTemplate {
  public NodeTemplate(Func<NodeModel> factory) {
    ArgumentNullException.ThrowIfNull(factory);
    Factory = factory;
    Instance = factory();
  }

  /// <summary>
  /// Factory function to create a new node instance of this template type.
  /// </summary>
  public Func<NodeModel> Factory { get; }

  /// <summary>
  /// Example node instance created by <see cref="Factory"/>.
  /// </summary>
  public NodeModel Instance { get; }
}
