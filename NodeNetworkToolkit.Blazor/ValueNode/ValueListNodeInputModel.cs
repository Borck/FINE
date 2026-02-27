namespace NodeNetwork.Toolkit.Blazor.ValueNode {
  using NodeNetwork.Blazor.Models;

  /// <summary>
  /// Typed multi-connection value input abstraction.
  /// </summary>
  public sealed class ValueListNodeInputModel<T> {
  /// <summary>
  /// Associated input port.
  /// </summary>
  public required NodePortModel Port { get; init; }

  /// <summary>
  /// Resolves current values from all connected outputs.
  /// </summary>
  public IReadOnlyList<T> GetValues(NetworkModel network, ValuePortRegistry registry) {
    ArgumentNullException.ThrowIfNull(network);
    ArgumentNullException.ThrowIfNull(registry);

    var values = new List<T>();
    foreach (var connection in network.Connections.Where(c => c.InputPortId == Port.Id)) {
      if (registry.TryGetOutput<T>(connection.OutputPortId, out var singleOutput) && singleOutput is not null) {
        values.Add(singleOutput.CurrentValue);
        continue;
      }

      if (registry.TryGetOutput<IReadOnlyList<T>>(connection.OutputPortId, out var listOutput) && listOutput is not null) {
        values.AddRange(listOutput.CurrentValue);
      }
    }

    return values;
  }
  }
}
