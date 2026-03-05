namespace FINE.Toolkit.Blazor.ContextMenu;

/// <summary>
/// Represents a partially defined connection used by add-node workflows.
/// </summary>
public sealed class PendingConnectionModel {
  /// <summary>
  /// Gets or sets the pending input endpoint id.
  /// </summary>
  public Guid? InputPortId { get; set; }

  /// <summary>
  /// Gets or sets the pending output endpoint id.
  /// </summary>
  public Guid? OutputPortId { get; set; }

  /// <summary>
  /// Indicates that the input endpoint is already fixed.
  /// </summary>
  public bool InputIsLocked { get; set; }

  /// <summary>
  /// Indicates that the output endpoint is already fixed.
  /// </summary>
  public bool OutputIsLocked { get; set; }
}
