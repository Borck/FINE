using NodeNetwork.Blazor.Models;
using NodeNetwork.Blazor.Validation;

namespace NodeNetwork.Blazor.Services;

/// <summary>
/// Stateful editor logic separated from rendering so both server-side and WebAssembly Blazor can use it.
/// </summary>
public sealed class NetworkEditorState
{
    private readonly INetworkConnectionValidator _connectionValidator;

    public NetworkEditorState(NetworkModel network, INetworkConnectionValidator? connectionValidator = null)
    {
        Network = network;
        _connectionValidator = connectionValidator ?? new DefaultNetworkConnectionValidator();
    }

    public event Action? Changed;

    public NetworkModel Network { get; }

    public Guid? PendingPortId { get; private set; }

    public string? LastError { get; private set; }

    public bool MultiSelectEnabled { get; set; }

    public void AddNode(NodeModel node)
    {
        Network.Nodes.Add(node);
        NotifyChanged();
    }

    public void RemoveNode(Guid nodeId)
    {
        var node = Network.Nodes.FirstOrDefault(n => n.Id == nodeId);
        if (node is null || !node.CanBeRemovedByUser)
        {
            return;
        }

        var portIds = node.GetPorts().Select(p => p.Id).ToHashSet();
        Network.Connections.RemoveAll(c => portIds.Contains(c.InputPortId) || portIds.Contains(c.OutputPortId));
        Network.Nodes.Remove(node);

        NotifyChanged();
    }

    public void MoveNode(Guid nodeId, CanvasPoint position)
    {
        var node = Network.Nodes.FirstOrDefault(n => n.Id == nodeId);
        if (node is null)
        {
            return;
        }

        node.Position = position;
        NotifyChanged();
    }

    public void SetNodeSelected(Guid nodeId, bool selected)
    {
        var node = Network.Nodes.FirstOrDefault(n => n.Id == nodeId);
        if (node is null)
        {
            return;
        }

        if (!MultiSelectEnabled && selected)
        {
            foreach (var n in Network.Nodes)
            {
                n.IsSelected = false;
            }
        }

        node.IsSelected = selected;
        NotifyChanged();
    }

    public void DeleteSelectedNodes()
    {
        var selectedIds = Network.Nodes.Where(n => n.IsSelected).Select(n => n.Id).ToArray();
        foreach (var id in selectedIds)
        {
            RemoveNode(id);
        }
    }

    public void Zoom(double delta)
    {
        Network.Zoom = Math.Clamp(Network.Zoom + delta, 0.25, 2.5);
        NotifyChanged();
    }

    public void Pan(CanvasPoint delta)
    {
        Network.Pan = Network.Pan + delta;
        NotifyChanged();
    }

    public void BeginConnection(Guid portId)
    {
        PendingPortId = portId;
        LastError = null;
        NotifyChanged();
    }

    public void CancelPendingConnection()
    {
        PendingPortId = null;
        NotifyChanged();
    }

    public ConnectionValidationResult TryCompleteConnection(Guid portId)
    {
        LastError = null;

        if (PendingPortId is null)
        {
            BeginConnection(portId);
            return ConnectionValidationResult.Allowed();
        }

        var first = FindPort(PendingPortId.Value);
        var second = FindPort(portId);
        PendingPortId = null;

        if (first is null || second is null)
        {
            return SetError("Port not found.");
        }

        var validation = _connectionValidator.Validate(Network, first, second);
        if (!validation.IsAllowed)
        {
            return SetError(validation.Error ?? "Invalid connection.");
        }

        var output = first.Direction == PortDirection.Output ? first : second;
        var input = first.Direction == PortDirection.Input ? first : second;

        Network.Connections.Add(new ConnectionModel
        {
            OutputPortId = output.Id,
            InputPortId = input.Id
        });

        NotifyChanged();
        return validation;
    }

    public void RemoveConnection(Guid connectionId)
    {
        Network.Connections.RemoveAll(c => c.Id == connectionId);
        NotifyChanged();
    }

    public NodePortModel? FindPort(Guid portId)
    {
        return Network.Nodes.SelectMany(n => n.GetPorts()).FirstOrDefault(p => p.Id == portId);
    }

    private ConnectionValidationResult SetError(string message)
    {
        LastError = message;
        NotifyChanged();
        return ConnectionValidationResult.Rejected(message);
    }

    private void NotifyChanged() => Changed?.Invoke();
}
