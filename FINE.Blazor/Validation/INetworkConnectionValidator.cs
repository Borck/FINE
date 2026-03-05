namespace FINE.Blazor.Validation;

using FINE.Blazor.Models;

public interface INetworkConnectionValidator {
  ConnectionValidationResult Validate(NetworkModel network, NodePortModel source, NodePortModel target);
}
