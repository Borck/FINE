namespace FINE.Toolkit.Blazor.ValueNode;

/// <summary>
/// Registry that maps output port ids to typed value outputs.
/// </summary>
public sealed class ValuePortRegistry {
  private readonly Dictionary<Guid, object> _outputs = [];

  /// <summary>
  /// Registers or replaces an output mapping.
  /// </summary>
  public void RegisterOutput<T>(ValueNodeOutputModel<T> output) {
    ArgumentNullException.ThrowIfNull(output);
    _outputs[output.Port.Id] = output;
  }

  /// <summary>
  /// Removes an output mapping.
  /// </summary>
  public bool UnregisterOutput(Guid portId) => _outputs.Remove(portId);

  /// <summary>
  /// Resolves a typed output for a port id.
  /// </summary>
  public bool TryGetOutput<T>(Guid portId, out ValueNodeOutputModel<T>? output) {
    if (_outputs.TryGetValue(portId, out var value) && value is ValueNodeOutputModel<T> typed) {
      output = typed;
      return true;
    }

    output = null;
    return false;
  }
}
