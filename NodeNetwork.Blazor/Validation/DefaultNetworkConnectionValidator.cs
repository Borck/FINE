using NodeNetwork.Blazor.Models;

namespace NodeNetwork.Blazor.Validation;

/// <summary>
/// Enforces the same baseline constraints as classic NodeNetwork:
/// direction, self-loop prevention, type compatibility, and capacity.
/// </summary>
public sealed class DefaultNetworkConnectionValidator : INetworkConnectionValidator
{
    public ConnectionValidationResult Validate(NetworkModel network, NodePortModel source, NodePortModel target)
    {
        if (source.Direction == target.Direction)
        {
            return ConnectionValidationResult.Rejected("Connection must be between an output and an input port.");
        }

        var output = source.Direction == PortDirection.Output ? source : target;
        var input = source.Direction == PortDirection.Input ? source : target;

        if (output.NodeId == input.NodeId)
        {
            return ConnectionValidationResult.Rejected("Connection inside the same node is not allowed.");
        }

        if (!string.Equals(output.DataType, input.DataType, StringComparison.Ordinal))
        {
            return ConnectionValidationResult.Rejected($"Incompatible data types: '{output.DataType}' and '{input.DataType}'.");
        }

        var duplicate = network.Connections.Any(c => c.OutputPortId == output.Id && c.InputPortId == input.Id);
        if (duplicate)
        {
            return ConnectionValidationResult.Rejected("Connection already exists.");
        }

        if (input.Capacity == PortCapacity.Single && network.Connections.Any(c => c.InputPortId == input.Id))
        {
            return ConnectionValidationResult.Rejected("Input port only accepts a single connection.");
        }

        if (output.Capacity == PortCapacity.Single && network.Connections.Any(c => c.OutputPortId == output.Id))
        {
            return ConnectionValidationResult.Rejected("Output port only accepts a single connection.");
        }

        return ConnectionValidationResult.Allowed();
    }
}
