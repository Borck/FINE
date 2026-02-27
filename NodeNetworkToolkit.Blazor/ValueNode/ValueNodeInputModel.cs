namespace NodeNetwork.Toolkit.Blazor.ValueNode {
  using NodeNetwork.Blazor.Models;
  using NodeNetwork.Blazor.Validation;

  /// <summary>
  /// Typed single-connection value input abstraction.
  /// </summary>
  public sealed class ValueNodeInputModel<T> {
  /// <summary>
  /// Associated input port.
  /// </summary>
  public required NodePortModel Port { get; init; }

  /// <summary>
  /// Editor fallback used when the input has no valid incoming connection.
  /// </summary>
  public ValueEditorModel<T>? Editor { get; set; }

  /// <summary>
  /// Validation behavior when connection topology changes.
  /// </summary>
  public ValidationAction ConnectionChangedValidationAction { get; init; } = ValidationAction.PushDefaultValue;

  /// <summary>
  /// Validation behavior when a connected output value changes.
  /// </summary>
  public ValidationAction ConnectedValueChangedValidationAction { get; init; } = ValidationAction.IgnoreValidation;

  /// <summary>
  /// Resolves the effective input value from network connections and fallback editor state.
  /// </summary>
  public T GetValue(NetworkModel network, ValuePortRegistry registry, INetworkConnectionValidator? validator = null) {
    ArgumentNullException.ThrowIfNull(network);
    ArgumentNullException.ThrowIfNull(registry);

    var connection = network.Connections.FirstOrDefault(c => c.InputPortId == Port.Id);
    if (connection is null) {
      return Editor is null ? default! : Editor.Value;
    }

    if (!registry.TryGetOutput<T>(connection.OutputPortId, out var output) || output is null) {
      return default!;
    }

    if (ConnectionChangedValidationAction != ValidationAction.DontValidate ||
        ConnectedValueChangedValidationAction != ValidationAction.DontValidate) {
      var resolvedValidator = validator ?? new DefaultNetworkConnectionValidator();
      var sourcePort = FindPort(network, connection.OutputPortId);
      var validation = resolvedValidator.Validate(network, sourcePort, Port);

      if (!validation.IsAllowed &&
          (ConnectionChangedValidationAction == ValidationAction.PushDefaultValue ||
           ConnectedValueChangedValidationAction == ValidationAction.PushDefaultValue)) {
        return default!;
      }
    }

    return output.CurrentValue;
  }

  private static NodePortModel FindPort(NetworkModel network, Guid portId) =>
    network.Nodes.SelectMany(node => node.GetPorts()).First(port => port.Id == portId);
  }
}
