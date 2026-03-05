namespace NodeNetwork.Toolkit.Blazor.Layout.ForceDirected;

using NodeNetwork.Blazor.Models;

/// <summary>
/// Compatibility configuration matching the original toolkit force-directed API.
/// </summary>
public sealed class Configuration {
  /// <summary>
  /// The network whose nodes are to be repositioned.
  /// </summary>
  public required NetworkModel Network { get; init; }

  /// <summary>
  /// Scales the elapsed simulation time.
  /// </summary>
  public float TimeModifier { get; set; } = 3.5f;

  /// <summary>
  /// Number of internal updates per iteration.
  /// </summary>
  public int UpdatesPerIteration { get; set; } = 1;

  /// <summary>
  /// Repulsive strength between nodes.
  /// </summary>
  public float NodeRepulsionForce { get; set; } = 100f;

  /// <summary>
  /// Per-connection equilibrium spring length.
  /// </summary>
  public Func<ConnectionModel, double> EquilibriumDistance { get; set; } = _ => 100d;

  /// <summary>
  /// Per-connection spring constant.
  /// </summary>
  public Func<ConnectionModel, double> SpringConstant { get; set; } = _ => 1d;

  /// <summary>
  /// Per-connection row force that tends to place source nodes left of target nodes.
  /// </summary>
  public Func<ConnectionModel, double> RowForce { get; set; } = _ => 100d;

  /// <summary>
  /// Per-node simulation mass.
  /// </summary>
  public Func<NodeModel, float> NodeMass { get; set; } = _ => 10f;

  /// <summary>
  /// Per-node friction coefficient.
  /// </summary>
  public Func<NodeModel, float> FrictionCoefficient { get; set; } = _ => 2.5f;

  /// <summary>
  /// Indicates whether node position is fixed and should not be moved.
  /// </summary>
  public Func<NodeModel, bool> IsFixedNode { get; set; } = _ => false;
}
