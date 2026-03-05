namespace NodeNetwork.Blazor.Compatibility;

using NodeNetwork.Blazor.Models;

public interface INodeNetworkCompatibilityAdapter {
  NodeNetworkDocument Export(NetworkModel network);

  NetworkModel Import(NodeNetworkDocument document);
}
