namespace FINE.Toolkit.Blazor.ValueNode;

/// <summary>
/// Defines how value ports should react to validation outcomes.
/// </summary>
public enum ValidationAction {
  DontValidate,
  IgnoreValidation,
  WaitForValid,
  PushDefaultValue
}
