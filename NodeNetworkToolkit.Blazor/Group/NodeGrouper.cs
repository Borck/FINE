namespace NodeNetwork.Toolkit.Blazor.Group;

using NodeNetwork.Blazor.Models;
using NodeNetwork.Toolkit.Blazor.Graph;

/// <summary>
/// Provides grouping/ungrouping of node sets into sub-networks.
/// </summary>
public sealed class NodeGrouper {
  /// <summary>
  /// Constructs a new group node hosted in the super network.
  /// </summary>
  public Func<NetworkModel, NodeModel> GroupNodeFactory { get; set; } = _ => new NodeModel { Name = "Group" };

  /// <summary>
  /// Constructs the subnet that will contain grouped nodes.
  /// </summary>
  public Func<NetworkModel> SubNetworkFactory { get; set; } = () => new NetworkModel();

  /// <summary>
  /// Constructs the entry node in the subnet.
  /// </summary>
  public Func<NodeModel> EntranceNodeFactory { get; set; } = () => new NodeModel { Name = "In" };

  /// <summary>
  /// Constructs the exit node in the subnet.
  /// </summary>
  public Func<NodeModel> ExitNodeFactory { get; set; } = () => new NodeModel { Name = "Out" };

  /// <summary>
  /// Constructs endpoint mapping between group node and subnet boundary nodes.
  /// </summary>
  public Func<NetworkModel, NetworkModel, NodeModel, NodeModel, NodeModel, NodeGroupIOBinding> IOBindingFactory { get; set; }
      = static (super, sub, group, entrance, exit) => new NodeGroupIOBinding(super, sub, group, entrance, exit);

  public NodeGroupIOBinding? MergeIntoGroup(NetworkModel network, IEnumerable<NodeModel> nodesToGroup) {
    ArgumentNullException.ThrowIfNull(network);
    ArgumentNullException.ThrowIfNull(nodesToGroup);

    var groupNodesSet = nodesToGroup as HashSet<NodeModel> ?? [.. nodesToGroup];
    if (groupNodesSet.Count == 0 || !GraphAlgorithms.IsContinuousSubGraphSet(network, groupNodesSet)) {
      return null;
    }

    var subnet = SubNetworkFactory();
    var groupNode = GroupNodeFactory(subnet);
    var entrance = EntranceNodeFactory();
    var exit = ExitNodeFactory();

    subnet.Nodes.Add(entrance);
    subnet.Nodes.Add(exit);
    network.Nodes.Add(groupNode);

    groupNode.Position = new CanvasPoint(groupNodesSet.Average(n => n.Position.X), groupNodesSet.Average(n => n.Position.Y));

    var avgY = groupNodesSet.Average(n => n.Position.Y);
    entrance.Position = new CanvasPoint(groupNodesSet.Min(n => n.Position.X) - 100, avgY);
    exit.Position = new CanvasPoint(groupNodesSet.Max(n => n.Position.X) + 100, avgY);

    var io = IOBindingFactory(network, subnet, groupNode, entrance, exit);

    var groupedIds = groupNodesSet.Select(n => n.Id).ToHashSet();
    var subnetConnections = new List<ConnectionModel>();
    var borderInputConnections = new List<ConnectionModel>();
    var borderOutputConnections = new List<ConnectionModel>();

    var portNodeMap = BuildPortNodeMap(network);
    foreach (var con in network.Connections) {
      if (!portNodeMap.TryGetValue(con.InputPortId, out var inputNodeId) ||
          !portNodeMap.TryGetValue(con.OutputPortId, out var outputNodeId)) {
        continue;
      }

      var inputIn = groupedIds.Contains(inputNodeId);
      var outputIn = groupedIds.Contains(outputNodeId);

      if (inputIn && outputIn) {
        subnetConnections.Add(con);
      } else if (inputIn) {
        borderInputConnections.Add(con);
      } else if (outputIn) {
        borderOutputConnections.Add(con);
      }
    }

    var groupInputs = new Dictionary<Guid, NodePortModel>();
    var groupOutputs = new Dictionary<Guid, NodePortModel>();

    foreach (var border in borderInputConnections) {
      if (!groupInputs.ContainsKey(border.InputPortId)) {
        var sourceOutput = FindPort(network, border.OutputPortId);
        groupInputs[border.InputPortId] = io.AddNewGroupNodeInput(sourceOutput);
      }
    }

    foreach (var border in borderOutputConnections) {
      if (!groupOutputs.ContainsKey(border.OutputPortId)) {
        var sourceInput = FindPort(network, border.InputPortId);
        groupOutputs[border.OutputPortId] = io.AddNewGroupNodeOutput(sourceInput);
      }
    }

    network.Connections.RemoveAll(c => subnetConnections.Contains(c) || borderInputConnections.Contains(c) || borderOutputConnections.Contains(c));
    network.Nodes.RemoveAll(n => groupedIds.Contains(n.Id));

    subnet.Nodes.AddRange(groupNodesSet);
    subnet.Connections.AddRange(subnetConnections.Select(CloneConnection));

    network.Connections.AddRange(borderInputConnections.Select(con => new ConnectionModel {
      OutputPortId = con.OutputPortId,
      InputPortId = groupInputs[con.InputPortId].Id
    }));
    network.Connections.AddRange(borderOutputConnections.Select(con => new ConnectionModel {
      OutputPortId = groupOutputs[con.OutputPortId].Id,
      InputPortId = con.InputPortId
    }));

    subnet.Connections.AddRange(borderInputConnections.Select(con => new ConnectionModel {
      OutputPortId = io.GetSubnetInlet(groupInputs[con.InputPortId]).Id,
      InputPortId = con.InputPortId
    }));
    subnet.Connections.AddRange(borderOutputConnections.Select(con => new ConnectionModel {
      OutputPortId = con.OutputPortId,
      InputPortId = io.GetSubnetOutlet(groupOutputs[con.OutputPortId]).Id
    }));

    return io;
  }

  public void Ungroup(NodeGroupIOBinding binding) {
    ArgumentNullException.ThrowIfNull(binding);

    var super = binding.SuperNetwork;
    var sub = binding.SubNetwork;

    var boundaryNodeIds = new HashSet<Guid> { binding.EntranceNode.Id, binding.ExitNode.Id };

    var borderInputConnections = new List<(Guid OutputPortId, Guid[] InputPortIds)>();
    var borderOutputConnections = new List<(Guid InputPortId, Guid[] OutputPortIds)>();
    var innerConnections = new List<ConnectionModel>();

    foreach (var conn in sub.Connections) {
      var inputOwner = FindOwnerNode(sub, conn.InputPortId);
      var outputOwner = FindOwnerNode(sub, conn.OutputPortId);

      if (inputOwner == binding.EntranceNode.Id || inputOwner == binding.ExitNode.Id) {
        var groupOutput = binding.GetGroupNodeOutput(FindPort(sub, conn.InputPortId));
        var externalTargets = super.Connections
            .Where(c => c.OutputPortId == groupOutput.Id)
            .Select(c => c.InputPortId)
            .ToArray();

        if (externalTargets.Length > 0) {
          borderInputConnections.Add((conn.OutputPortId, externalTargets));
        }
      } else if (outputOwner == binding.EntranceNode.Id || outputOwner == binding.ExitNode.Id) {
        var groupInput = binding.GetGroupNodeInput(FindPort(sub, conn.OutputPortId));
        var externalSources = super.Connections
            .Where(c => c.InputPortId == groupInput.Id)
            .Select(c => c.OutputPortId)
            .ToArray();

        if (externalSources.Length > 0) {
          borderOutputConnections.Add((conn.InputPortId, externalSources));
        }
      } else {
        innerConnections.Add(conn);
      }
    }

    var memberNodes = sub.Nodes.Where(n => !boundaryNodeIds.Contains(n.Id)).ToArray();
    if (memberNodes.Length == 0) {
      return;
    }

    var minX = memberNodes.Min(n => n.Position.X);
    var minY = memberNodes.Min(n => n.Position.Y);
    var maxX = memberNodes.Max(n => n.Position.X);
    var maxY = memberNodes.Max(n => n.Position.Y);
    var center = new CanvasPoint(minX + (maxX - minX) / 2d, minY + (maxY - minY) / 2d);

    sub.Connections.Clear();
    sub.Nodes.Clear();

    var groupPos = binding.GroupNode.Position;
    super.Nodes.RemoveAll(n => n.Id == binding.GroupNode.Id);
    super.Connections.RemoveAll(c => {
      var inputOwner = FindOwnerNode(super, c.InputPortId);
      var outputOwner = FindOwnerNode(super, c.OutputPortId);
      return inputOwner == binding.GroupNode.Id || outputOwner == binding.GroupNode.Id;
    });

    super.Nodes.AddRange(memberNodes);
    foreach (var node in memberNodes) {
      node.Position = new CanvasPoint(node.Position.X - center.X + groupPos.X, node.Position.Y - center.Y + groupPos.Y);
    }

    super.Connections.AddRange(innerConnections.Select(CloneConnection));

    foreach (var (outputPortId, inputPortIds) in borderInputConnections) {
      super.Connections.AddRange(inputPortIds.Select(inputPortId => new ConnectionModel {
        OutputPortId = outputPortId,
        InputPortId = inputPortId
      }));
    }

    foreach (var (inputPortId, outputPortIds) in borderOutputConnections) {
      super.Connections.AddRange(outputPortIds.Select(outputPortId => new ConnectionModel {
        OutputPortId = outputPortId,
        InputPortId = inputPortId
      }));
    }
  }

  private static ConnectionModel CloneConnection(ConnectionModel connection) => new ConnectionModel {
    OutputPortId = connection.OutputPortId,
    InputPortId = connection.InputPortId
  };

  private static Dictionary<Guid, Guid> BuildPortNodeMap(NetworkModel network) {
    var map = new Dictionary<Guid, Guid>();
    foreach (var node in network.Nodes) {
      foreach (var port in node.GetPorts()) {
        map[port.Id] = node.Id;
      }
    }

    return map;
  }

  private static NodePortModel FindPort(NetworkModel network, Guid portId) => network.Nodes.SelectMany(n => n.GetPorts()).First(p => p.Id == portId);

  private static Guid FindOwnerNode(NetworkModel network, Guid portId) => network.Nodes.First(node => node.GetPorts().Any(port => port.Id == portId)).Id;
}
