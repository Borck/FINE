namespace NodeNetwork.Blazor.Models;

public sealed class ConnectionModel {
  public Guid Id { get; init; } = Guid.NewGuid();

  public required Guid OutputPortId { get; init; }

  public required Guid InputPortId { get; init; }
}
