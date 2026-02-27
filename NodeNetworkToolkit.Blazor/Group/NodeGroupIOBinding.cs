namespace NodeNetwork.Toolkit.Blazor.Group;

using NodeNetwork.Blazor.Models;

/// <summary>
/// Provides bidirectional endpoint mapping between a super-network group node and a subnet entrance/exit pair.
/// </summary>
public sealed class NodeGroupIOBinding(NetworkModel superNetwork, NetworkModel subNetwork, NodeModel groupNode, NodeModel entranceNode, NodeModel exitNode) {
  private readonly Dictionary<Guid, Guid> _groupInputToSubnetInlet = [];
  private readonly Dictionary<Guid, Guid> _groupOutputToSubnetOutlet = [];

  public NetworkModel SuperNetwork { get; } = superNetwork ?? throw new ArgumentNullException(nameof(superNetwork));

  public NetworkModel SubNetwork { get; } = subNetwork ?? throw new ArgumentNullException(nameof(subNetwork));

  public NodeModel GroupNode { get; } = groupNode ?? throw new ArgumentNullException(nameof(groupNode));

  public NodeModel EntranceNode { get; } = entranceNode ?? throw new ArgumentNullException(nameof(entranceNode));

  public NodeModel ExitNode { get; } = exitNode ?? throw new ArgumentNullException(nameof(exitNode));

  public NodePortModel AddNewGroupNodeInput(NodePortModel candidateOutput) {
    var groupInput = CreateCompatibleInput(GroupNode, candidateOutput, GroupNode.Inputs);
    GroupNode.Inputs.Add(groupInput);

    var subnetInlet = CreateCompatibleOutput(EntranceNode, candidateOutput, EntranceNode.Outputs);
    EntranceNode.Outputs.Add(subnetInlet);

    _groupInputToSubnetInlet[groupInput.Id] = subnetInlet.Id;
    return groupInput;
  }

  public NodePortModel AddNewGroupNodeOutput(NodePortModel candidateInput) {
    var groupOutput = CreateCompatibleOutput(GroupNode, candidateInput, GroupNode.Outputs);
    GroupNode.Outputs.Add(groupOutput);

    var subnetOutlet = CreateCompatibleInput(ExitNode, candidateInput, ExitNode.Inputs);
    ExitNode.Inputs.Add(subnetOutlet);

    _groupOutputToSubnetOutlet[groupOutput.Id] = subnetOutlet.Id;
    return groupOutput;
  }

  public NodePortModel GetSubnetInlet(NodePortModel groupNodeInput) =>
      FindPort(EntranceNode, _groupInputToSubnetInlet[groupNodeInput.Id]);

  public NodePortModel GetSubnetOutlet(NodePortModel groupNodeOutput) =>
      FindPort(ExitNode, _groupOutputToSubnetOutlet[groupNodeOutput.Id]);

  public NodePortModel GetGroupNodeInput(NodePortModel subnetInlet) =>
      FindPortByReverseMap(GroupNode, _groupInputToSubnetInlet, subnetInlet.Id, GroupNode.Inputs);

  public NodePortModel GetGroupNodeOutput(NodePortModel subnetOutlet) =>
      FindPortByReverseMap(GroupNode, _groupOutputToSubnetOutlet, subnetOutlet.Id, GroupNode.Outputs);

  /// <summary>
  /// Deletes a mapped endpoint from either side of the group/subnet mapping.
  /// The corresponding mapped endpoint is removed as well.
  /// </summary>
  public void DeleteEndpoint(NodePortModel endpoint) {
    ArgumentNullException.ThrowIfNull(endpoint);

    if (_groupInputToSubnetInlet.Remove(endpoint.Id, out var subnetInletId)) {
      GroupNode.Inputs.RemoveAll(port => port.Id == endpoint.Id);
      EntranceNode.Outputs.RemoveAll(port => port.Id == subnetInletId);
      return;
    }

    if (_groupOutputToSubnetOutlet.Remove(endpoint.Id, out var subnetOutletId)) {
      GroupNode.Outputs.RemoveAll(port => port.Id == endpoint.Id);
      ExitNode.Inputs.RemoveAll(port => port.Id == subnetOutletId);
      return;
    }

    var groupInput = _groupInputToSubnetInlet.FirstOrDefault(pair => pair.Value == endpoint.Id);
    if (groupInput.Key != Guid.Empty) {
      _groupInputToSubnetInlet.Remove(groupInput.Key);
      GroupNode.Inputs.RemoveAll(port => port.Id == groupInput.Key);
      EntranceNode.Outputs.RemoveAll(port => port.Id == endpoint.Id);
      return;
    }

    var groupOutput = _groupOutputToSubnetOutlet.FirstOrDefault(pair => pair.Value == endpoint.Id);
    if (groupOutput.Key != Guid.Empty) {
      _groupOutputToSubnetOutlet.Remove(groupOutput.Key);
      GroupNode.Outputs.RemoveAll(port => port.Id == groupOutput.Key);
      ExitNode.Inputs.RemoveAll(port => port.Id == endpoint.Id);
    }
  }

  private static NodePortModel CreateCompatibleInput(NodeModel owner, NodePortModel candidateOutput, IReadOnlyCollection<NodePortModel> existing) => new NodePortModel {
    Id = Guid.NewGuid(),
    NodeId = owner.Id,
    Key = candidateOutput.Key,
    DisplayName = candidateOutput.DisplayName,
    DataType = candidateOutput.DataType,
    Direction = PortDirection.Input,
    Capacity = PortCapacity.Multi,
    SortIndex = existing.Count,
    IsVisible = true
  };

  private static NodePortModel CreateCompatibleOutput(NodeModel owner, NodePortModel candidateInput, IReadOnlyCollection<NodePortModel> existing) => new NodePortModel {
    Id = Guid.NewGuid(),
    NodeId = owner.Id,
    Key = candidateInput.Key,
    DisplayName = candidateInput.DisplayName,
    DataType = candidateInput.DataType,
    Direction = PortDirection.Output,
    Capacity = PortCapacity.Multi,
    SortIndex = existing.Count,
    IsVisible = true
  };

  private static NodePortModel FindPort(NodeModel node, Guid id) => node.GetPorts().First(port => port.Id == id);

  private static NodePortModel FindPortByReverseMap(
      NodeModel node,
      IReadOnlyDictionary<Guid, Guid> mapping,
      Guid value,
      IEnumerable<NodePortModel> expectedPorts) {
    var key = mapping.First(pair => pair.Value == value).Key;
    return expectedPorts.First(port => port.Id == key);
  }
}
