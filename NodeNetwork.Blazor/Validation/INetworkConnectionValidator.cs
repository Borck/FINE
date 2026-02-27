using NodeNetwork.Blazor.Models;

namespace NodeNetwork.Blazor.Validation;

public interface INetworkConnectionValidator
{
    ConnectionValidationResult Validate(NetworkModel network, NodePortModel source, NodePortModel target);
}
