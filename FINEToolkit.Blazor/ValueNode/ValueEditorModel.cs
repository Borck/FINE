namespace FINE.Toolkit.Blazor.ValueNode {
  /// <summary>
  /// Mutable value container used by value-port inputs/outputs.
  /// </summary>
  public sealed class ValueEditorModel<T> {
    /// <summary>
    /// Current value.
    /// </summary>
    public T Value { get; private set; } = default!;

    /// <summary>
    /// Raised whenever <see cref="Value"/> changes.
    /// </summary>
    public event Action<T>? ValueChanged;

    /// <summary>
    /// Updates the current value and notifies listeners.
    /// </summary>
    public void SetValue(T value) {
      Value = value;
      ValueChanged?.Invoke(value);
    }
  }
}
