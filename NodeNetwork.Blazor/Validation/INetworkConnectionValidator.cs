namespace NodeNetwork.Blazor.Validation;

using NodeNetwork.Blazor.Models;

public interface INetworkConnectionValidator {
  ConnectionValidationResult Validate(NetworkModel network, NodePortModel source, NodePortModel target);
}
