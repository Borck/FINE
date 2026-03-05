namespace NodeNetwork.Toolkit.Blazor.ValueNode {
  using NodeNetwork.Blazor.Models;

  /// <summary>
  /// Typed output abstraction that binds values to a specific output port.
  /// </summary>
  public sealed class ValueNodeOutputModel<T> {
    /// <summary>
    /// Associated output port.
    /// </summary>
    public required NodePortModel Port { get; init; }

    /// <summary>
    /// Latest output value.
    /// </summary>
    public T CurrentValue { get; private set; } = default!;

    /// <summary>
    /// Raised when <see cref="CurrentValue"/> changes.
    /// </summary>
    public event Action<T>? ValueChanged;

    /// <summary>
    /// Updates the output value and notifies listeners.
    /// </summary>
    public void SetValue(T value) {
      CurrentValue = value;
      ValueChanged?.Invoke(value);
    }
  }
}
