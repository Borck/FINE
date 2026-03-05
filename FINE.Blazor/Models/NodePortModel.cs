namespace FINE.Blazor.Models;

public sealed class NodePortModel {
  public required Guid Id { get; init; }

  public required Guid NodeId { get; init; }

  public required string Key { get; init; }

  public required string DisplayName { get; init; }

  public required string DataType { get; init; }

  public required PortDirection Direction { get; init; }

  public PortCapacity Capacity { get; init; } = PortCapacity.Multi;

  public int SortIndex { get; init; }

  public bool IsVisible { get; set; } = true;
}
