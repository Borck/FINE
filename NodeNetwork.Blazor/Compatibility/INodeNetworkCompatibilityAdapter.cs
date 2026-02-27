using NodeNetwork.Blazor.Models;

namespace NodeNetwork.Blazor.Compatibility;

public interface INodeNetworkCompatibilityAdapter
{
    NodeNetworkDocument Export(NetworkModel network);

    NetworkModel Import(NodeNetworkDocument document);
}
