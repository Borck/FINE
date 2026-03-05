namespace NodeNetwork.Toolkit.Layout.ForceDirected;

using System.Collections.Generic;
using System.Windows;
using NodeNetwork.ViewModels;

internal interface IState {
  Vector GetNodePosition(NodeViewModel node);
  void SetNodePosition(NodeViewModel node, Vector pos);
  Vector GetEndpointPosition(Endpoint endpoint);
  Vector GetNodeSpeed(NodeViewModel node);
  void SetNodeSpeed(NodeViewModel node, Vector speed);
}

internal class BufferedState : IState {
  private readonly Dictionary<NodeViewModel, Vector> _nodePositions = new Dictionary<NodeViewModel, Vector>();
  private readonly Dictionary<Endpoint, Vector> _endpointRelativePositions = new Dictionary<Endpoint, Vector>();
  public IEnumerable<KeyValuePair<NodeViewModel, Vector>> NodePositions => _nodePositions;

  private readonly Dictionary<NodeViewModel, Vector> _nodeSpeeds = new Dictionary<NodeViewModel, Vector>();

  public Vector GetNodePosition(NodeViewModel node) {
    if (!_nodePositions.TryGetValue(node, out var result)) {
      result = new Vector(node.Position.X, node.Position.Y);
    }

    return result;
  }

  public void SetNodePosition(NodeViewModel node, Vector pos) => _nodePositions[node] = pos;

  public Vector GetEndpointPosition(Endpoint endpoint) {
    if (!_endpointRelativePositions.TryGetValue(endpoint, out var result)) {
      result = new Vector(endpoint.Port.CenterPoint.X, endpoint.Port.CenterPoint.Y) - GetNodePosition(endpoint.Parent);
      _endpointRelativePositions[endpoint] = result;
    }

    return result + GetNodePosition(endpoint.Parent);
  }

  public Vector GetNodeSpeed(NodeViewModel node) {
    if (!_nodeSpeeds.TryGetValue(node, out var result)) {
      result = new Vector(0, 0);
    }

    return result;
  }

  public void SetNodeSpeed(NodeViewModel node, Vector speed) => _nodeSpeeds[node] = speed;
}

internal class LiveState : IState {
  private readonly Dictionary<NodeViewModel, Vector> _nodeSpeeds = new Dictionary<NodeViewModel, Vector>();

  public Vector GetNodePosition(NodeViewModel node) => new Vector(node.Position.X, node.Position.Y);

  public void SetNodePosition(NodeViewModel node, Vector pos) => node.Position = new Point(pos.X, pos.Y);

  public Vector GetEndpointPosition(Endpoint endpoint) => new Vector(endpoint.Port.CenterPoint.X, endpoint.Port.CenterPoint.Y);

  public Vector GetNodeSpeed(NodeViewModel node) {
    if (!_nodeSpeeds.TryGetValue(node, out var result)) {
      result = new Vector(0, 0);
    }

    return result;
  }

  public void SetNodeSpeed(NodeViewModel node, Vector speed) => _nodeSpeeds[node] = speed;
}
