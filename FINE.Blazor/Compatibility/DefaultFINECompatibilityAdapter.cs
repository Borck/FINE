namespace FINE.Blazor.Compatibility;

using FINE.Blazor.Models;

/// <summary>
/// Converts between Blazor runtime models and a portable document contract.
/// The same contract can be used by a WPF adapter to switch implementations with minimal friction.
/// </summary>
public sealed class DefaultFINECompatibilityAdapter : IFINECompatibilityAdapter {
  public FINEDocument Export(NetworkModel network) => new FINEDocument {
    Nodes = network.Nodes.Select(n => new NodeDocument {
      Id = n.Id,
      Name = n.Name,
      X = n.Position.X,
      Y = n.Position.Y,
      Width = n.Size.Width,
      Height = n.Size.Height,
      IsCollapsed = n.IsCollapsed,
      CanBeRemovedByUser = n.CanBeRemovedByUser,
      Inputs = n.Inputs.Select(ToDocument).ToList(),
      Outputs = n.Outputs.Select(ToDocument).ToList()
    }).ToList(),
    Connections = network.Connections.Select(c => new ConnectionDocument {
      Id = c.Id,
      OutputPortId = c.OutputPortId,
      InputPortId = c.InputPortId
    }).ToList()
  };

  public NetworkModel Import(FINEDocument document) {
    var network = new NetworkModel();

    foreach (var node in document.Nodes) {
      var model = new NodeModel {
        Id = node.Id,
        Name = node.Name,
        Position = new CanvasPoint(node.X, node.Y),
        Size = new CanvasSize(node.Width, node.Height),
        IsCollapsed = node.IsCollapsed,
        CanBeRemovedByUser = node.CanBeRemovedByUser
      };

      model.Inputs.AddRange(node.Inputs.Select(p => FromDocument(node.Id, p, PortDirection.Input)));
      model.Outputs.AddRange(node.Outputs.Select(p => FromDocument(node.Id, p, PortDirection.Output)));
      network.Nodes.Add(model);
    }

    network.Connections.AddRange(document.Connections.Select(c => new ConnectionModel {
      Id = c.Id,
      OutputPortId = c.OutputPortId,
      InputPortId = c.InputPortId
    }));

    return network;
  }

  private static PortDocument ToDocument(NodePortModel port) => new PortDocument {
    Id = port.Id,
    Key = port.Key,
    DisplayName = port.DisplayName,
    DataType = port.DataType,
    IsInput = port.Direction == PortDirection.Input,
    IsMulti = port.Capacity == PortCapacity.Multi,
    SortIndex = port.SortIndex,
    IsVisible = port.IsVisible
  };

  private static NodePortModel FromDocument(Guid nodeId, PortDocument port, PortDirection fallbackDirection) => new NodePortModel {
    Id = port.Id,
    NodeId = nodeId,
    Key = port.Key,
    DisplayName = port.DisplayName,
    DataType = port.DataType,
    Direction = port.IsInput ? PortDirection.Input : fallbackDirection,
    Capacity = port.IsMulti ? PortCapacity.Multi : PortCapacity.Single,
    SortIndex = port.SortIndex,
    IsVisible = port.IsVisible
  };
}
