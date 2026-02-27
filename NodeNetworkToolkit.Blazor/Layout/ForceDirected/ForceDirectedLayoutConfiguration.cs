namespace NodeNetwork.Toolkit.Blazor.Layout.ForceDirected;

/// <summary>
/// Configuration options for <see cref="ForceDirectedLayouter"/>.
/// </summary>
public sealed class ForceDirectedLayoutConfiguration {
  /// <summary>
  /// Gets or sets the number of simulation iterations.
  /// </summary>
  public int Iterations { get; set; } = 400;

  /// <summary>
  /// Gets or sets the repulsive force strength between all node pairs.
  /// </summary>
  public double RepulsionStrength { get; set; } = 20_000d;

  /// <summary>
  /// Gets or sets the spring force strength used for connected node pairs.
  /// </summary>
  public double SpringStrength { get; set; } = 0.005d;

  /// <summary>
  /// Gets or sets the target spring length for connected node pairs.
  /// </summary>
  public double SpringLength { get; set; } = 220d;

  /// <summary>
  /// Gets or sets the velocity damping factor. Valid range is 0 to 1.
  /// </summary>
  public double Damping { get; set; } = 0.85d;

  /// <summary>
  /// Gets or sets the center attraction coefficient pulling nodes towards the origin.
  /// </summary>
  public double CenterAttraction { get; set; } = 0.002d;

  /// <summary>
  /// Gets or sets the maximum movement per iteration.
  /// </summary>
  public double MaxStep { get; set; } = 24d;

  /// <summary>
  /// Gets or sets the jitter amount for resolving overlapping initial positions.
  /// </summary>
  public double InitialJitter { get; set; } = 1d;
}
