namespace NodeNetwork.Blazor.Compatibility;

/// <summary>
/// UI-independent graph document used to switch between WPF NodeNetwork and Blazor NodeNetwork.
/// </summary>
public sealed class NodeNetworkDocument
{
    public List<NodeDocument> Nodes { get; init; } = [];

    public List<ConnectionDocument> Connections { get; init; } = [];
}

public sealed class NodeDocument
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required double X { get; init; }

    public required double Y { get; init; }

    public required double Width { get; init; }

    public required double Height { get; init; }

    public bool IsCollapsed { get; init; }

    public bool CanBeRemovedByUser { get; init; }

    public List<PortDocument> Inputs { get; init; } = [];

    public List<PortDocument> Outputs { get; init; } = [];
}

public sealed class PortDocument
{
    public required Guid Id { get; init; }

    public required string Key { get; init; }

    public required string DisplayName { get; init; }

    public required string DataType { get; init; }

    public required bool IsInput { get; init; }

    public required bool IsMulti { get; init; }

    public int SortIndex { get; init; }

    public bool IsVisible { get; init; }
}

public sealed class ConnectionDocument
{
    public required Guid Id { get; init; }

    public required Guid OutputPortId { get; init; }

    public required Guid InputPortId { get; init; }
}
