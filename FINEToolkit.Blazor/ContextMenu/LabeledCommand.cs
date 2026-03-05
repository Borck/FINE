namespace FINE.Toolkit.Blazor.ContextMenu;

/// <summary>
/// Command metadata used by toolkit context menu models.
/// </summary>
public sealed class LabeledCommand {
  /// <summary>
  /// Display label of the command.
  /// </summary>
  public required string Label { get; init; }

  /// <summary>
  /// Indicates whether the command is visible in filtered menu views.
  /// </summary>
  public bool Visible { get; set; } = true;

  /// <summary>
  /// Delegate executed when the command is invoked.
  /// </summary>
  public required Func<object?, Task> Command { get; init; }

  /// <summary>
  /// Optional command parameter.
  /// </summary>
  public object? CommandParameter { get; init; }

  /// <summary>
  /// Executes the command delegate.
  /// </summary>
  public Task ExecuteAsync() => Command(CommandParameter);
}
