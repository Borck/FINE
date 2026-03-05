namespace NodeNetwork.Toolkit.Blazor.Graph;

using NodeNetwork.Blazor.Models;

/// <summary>
/// Utility algorithms for traversing and analyzing a <see cref="NetworkModel"/>.
/// </summary>
public static class GraphAlgorithms {
  /// <summary>
  /// Finds all connections that are part of at least one directed loop.
  /// </summary>
  /// <param name="network">The network to analyze.</param>
  /// <returns>A read-only list of loop connections.</returns>
  public static IReadOnlyList<ConnectionModel> FindLoopConnections(NetworkModel network) {
    ArgumentNullException.ThrowIfNull(network);

    var nodeByPort = BuildNodeByPortMap(network);
    var outgoing = BuildOutgoingLookup(network, nodeByPort);

    var result = new List<ConnectionModel>();
    foreach (var connection in network.Connections) {
      if (!TryGetConnectedNodes(connection, nodeByPort, out var sourceNodeId, out var targetNodeId)) {
        continue;
      }

      if (CanReach(targetNodeId, sourceNodeId, outgoing)) {
        result.Add(connection);
      }
    }

    return result;
  }

  /// <summary>
  /// Compatibility alias for NodeNetwork.Toolkit.GraphAlgorithms.FindLoops.
  /// </summary>
  public static IReadOnlyList<ConnectionModel> FindLoops(NetworkModel network) => FindLoopConnections(network);

  /// <summary>
  /// Finds nodes without incoming connections from other nodes inside the same network.
  /// </summary>
  /// <param name="network">The network to inspect.</param>
  /// <returns>A read-only list of starting nodes.</returns>
  public static IReadOnlyList<NodeModel> FindStartingNodes(NetworkModel network) {
    ArgumentNullException.ThrowIfNull(network);

    var nodeByPort = BuildNodeByPortMap(network);
    var incomingCount = network.Nodes.ToDictionary(node => node.Id, static _ => 0);

    foreach (var connection in network.Connections) {
      if (!TryGetConnectedNodes(connection, nodeByPort, out _, out var targetNodeId)) {
        continue;
      }

      if (incomingCount.TryGetValue(targetNodeId, out var count)) {
        incomingCount[targetNodeId] = count + 1;
      }
    }

    return network.Nodes.Where(node => incomingCount[node.Id] == 0).ToArray();
  }

  /// <summary>
  /// Returns starting nodes inside a specific node subset.
  /// </summary>
  public static IReadOnlyList<NodeModel> FindStartingNodes(NetworkModel network, IEnumerable<NodeModel> nodeGroup) {
    ArgumentNullException.ThrowIfNull(network);
    ArgumentNullException.ThrowIfNull(nodeGroup);

    var group = nodeGroup is HashSet<NodeModel> set
      ? set
      : [.. nodeGroup];

    var nodeByPort = BuildNodeByPortMap(network);
    var incomingInGroup = group.ToDictionary(n => n.Id, static _ => 0);

    foreach (var connection in network.Connections) {
      if (!TryGetConnectedNodes(connection, nodeByPort, out var sourceId, out var targetId)) {
        continue;
      }

      if (incomingInGroup.TryGetValue(targetId, out var value) && incomingInGroup.ContainsKey(sourceId)) {
        incomingInGroup[targetId] = ++value;
      }
    }

    return group.Where(n => incomingInGroup[n.Id] == 0).ToArray();
  }

  /// <summary>
  /// Finds all nodes reachable from the specified source node.
  /// </summary>
  /// <param name="network">The network containing the source node.</param>
  /// <param name="sourceNodeId">The node id where the traversal starts.</param>
  /// <param name="includeInputs">When set to <see langword="true"/>, traverses incoming edges.</param>
  /// <param name="includeOutputs">When set to <see langword="true"/>, traverses outgoing edges.</param>
  /// <param name="includeSelf">When set to <see langword="true"/>, includes the source node in the result.</param>
  /// <returns>A read-only list of connected nodes in breadth-first order.</returns>
  public static IReadOnlyList<NodeModel> FindConnectedNodes(
    NetworkModel network,
    Guid sourceNodeId,
    bool includeInputs = true,
    bool includeOutputs = true,
    bool includeSelf = false) {
    ArgumentNullException.ThrowIfNull(network);

    var nodesById = network.Nodes.ToDictionary(node => node.Id);
    if (!nodesById.TryGetValue(sourceNodeId, out var value)) {
      return Array.Empty<NodeModel>();
    }

    var nodeByPort = BuildNodeByPortMap(network);
    var outgoing = BuildOutgoingLookup(network, nodeByPort);
    var incoming = BuildIncomingLookup(network, nodeByPort);

    var visited = new HashSet<Guid> { sourceNodeId };
    var queue = new Queue<Guid>();
    queue.Enqueue(sourceNodeId);

    var result = new List<NodeModel>();
    if (includeSelf) {
      result.Add(value);
    }

    while (queue.Count > 0) {
      var current = queue.Dequeue();

      if (includeOutputs && outgoing.TryGetValue(current, out var outputNeighbors)) {
        Enqueue(outputNeighbors, nodesById, visited, queue, result);
      }

      if (includeInputs && incoming.TryGetValue(current, out var inputNeighbors)) {
        Enqueue(inputNeighbors, nodesById, visited, queue, result);
      }
    }

    return result;
  }

  /// <summary>
  /// Compatibility equivalent of GetConnectedNodesTunneling.
  /// </summary>
  public static IReadOnlyList<NodeModel> GetConnectedNodesTunneling(
    NetworkModel network,
    NodeModel startingNode,
    bool includeInputs = true,
    bool includeOutputs = false,
    bool includeSelf = false) {
    ArgumentNullException.ThrowIfNull(startingNode);
    return FindConnectedNodes(network, startingNode.Id, includeInputs, includeOutputs, includeSelf);
  }

  /// <summary>
  /// Compatibility equivalent of GetConnectedNodesBubbling.
  /// </summary>
  public static IReadOnlyList<NodeModel> GetConnectedNodesBubbling(
    NetworkModel network,
    NodeModel startingNode,
    bool includeInputs = true,
    bool includeOutputs = false,
    bool includeSelf = false) {
    ArgumentNullException.ThrowIfNull(startingNode);
    return GetConnectedNodesTunneling(network, startingNode, includeInputs, includeOutputs, includeSelf)
      .Reverse()
      .ToArray();
  }

  /// <summary>
  /// Splits the network nodes into weakly connected subgraphs.
  /// </summary>
  /// <param name="network">The network to analyze.</param>
  /// <returns>A read-only list of subgraphs where each item is a read-only node list.</returns>
  public static IReadOnlyList<IReadOnlyList<NodeModel>> FindSubGraphs(NetworkModel network) {
    ArgumentNullException.ThrowIfNull(network);

    var nodesById = network.Nodes.ToDictionary(node => node.Id);
    var nodeByPort = BuildNodeByPortMap(network);
    var adjacency = BuildUndirectedLookup(network, nodeByPort);

    var unvisited = new HashSet<Guid>(nodesById.Keys);
    var groups = new List<IReadOnlyList<NodeModel>>();

    while (unvisited.Count > 0) {
      var root = unvisited.First();
      var component = new List<NodeModel>();
      var queue = new Queue<Guid>();
      queue.Enqueue(root);
      unvisited.Remove(root);

      while (queue.Count > 0) {
        var current = queue.Dequeue();
        component.Add(nodesById[current]);

        if (!adjacency.TryGetValue(current, out var neighbors)) {
          continue;
        }

        foreach (var neighbor in neighbors) {
          if (unvisited.Remove(neighbor)) {
            queue.Enqueue(neighbor);
          }
        }
      }

      groups.Add(component);
    }

    return groups;
  }

  /// <summary>
  /// Splits a specific node set into connected components.
  /// </summary>
  public static IReadOnlyList<IReadOnlyList<NodeModel>> FindSubGraphs(NetworkModel network, IEnumerable<NodeModel> nodes) {
    ArgumentNullException.ThrowIfNull(network);
    ArgumentNullException.ThrowIfNull(nodes);

    var allowed = nodes.ToDictionary(n => n.Id);
    var nodeByPort = BuildNodeByPortMap(network);
    var adjacency = BuildUndirectedLookup(network, nodeByPort);

    var unvisited = new HashSet<Guid>(allowed.Keys);
    var groups = new List<IReadOnlyList<NodeModel>>();

    while (unvisited.Count > 0) {
      var root = unvisited.First();
      var component = new List<NodeModel>();
      var queue = new Queue<Guid>();
      queue.Enqueue(root);
      unvisited.Remove(root);

      while (queue.Count > 0) {
        var current = queue.Dequeue();
        component.Add(allowed[current]);

        if (!adjacency.TryGetValue(current, out var neighbors)) {
          continue;
        }

        foreach (var neighbor in neighbors) {
          if (!allowed.ContainsKey(neighbor)) {
            continue;
          }

          if (unvisited.Remove(neighbor)) {
            queue.Enqueue(neighbor);
          }
        }
      }

      groups.Add(component);
    }

    return groups;
  }

  /// <summary>
  /// Returns true when every graph component touched by the supplied node set is fully contained in that set.
  /// </summary>
  public static bool IsContinuousSubGraphSet(NetworkModel network, IEnumerable<NodeModel> nodesInSubGraphSet) {
    ArgumentNullException.ThrowIfNull(network);
    ArgumentNullException.ThrowIfNull(nodesInSubGraphSet);

    var set = nodesInSubGraphSet.ToHashSet();
    if (set.Count == 0) {
      return false;
    }

    var subGraphs = FindSubGraphs(network);
    foreach (var graph in subGraphs) {
      var anyInSet = graph.Any(set.Contains);
      if (!anyInSet) {
        continue;
      }

      if (graph.Any(node => !set.Contains(node))) {
        return false;
      }
    }

    return true;
  }

  private static void Enqueue(
    IEnumerable<Guid> neighbors,
    IReadOnlyDictionary<Guid, NodeModel> nodesById,
    ISet<Guid> visited,
    Queue<Guid> queue,
    ICollection<NodeModel> result) {
    foreach (var neighbor in neighbors) {
      if (!visited.Add(neighbor)) {
        continue;
      }

      queue.Enqueue(neighbor);
      result.Add(nodesById[neighbor]);
    }
  }

  private static Dictionary<Guid, NodeModel> BuildNodeByPortMap(NetworkModel network) {
    var map = new Dictionary<Guid, NodeModel>();

    foreach (var node in network.Nodes) {
      foreach (var port in node.GetPorts()) {
        map[port.Id] = node;
      }
    }

    return map;
  }

  private static Dictionary<Guid, HashSet<Guid>> BuildOutgoingLookup(NetworkModel network, IReadOnlyDictionary<Guid, NodeModel> nodeByPort) {
    var outgoing = new Dictionary<Guid, HashSet<Guid>>();

    foreach (var connection in network.Connections) {
      if (!TryGetConnectedNodes(connection, nodeByPort, out var source, out var target)) {
        continue;
      }

      if (!outgoing.TryGetValue(source, out var neighbors)) {
        neighbors = [];
        outgoing[source] = neighbors;
      }

      neighbors.Add(target);
    }

    return outgoing;
  }

  private static Dictionary<Guid, HashSet<Guid>> BuildIncomingLookup(NetworkModel network, IReadOnlyDictionary<Guid, NodeModel> nodeByPort) {
    var incoming = new Dictionary<Guid, HashSet<Guid>>();

    foreach (var connection in network.Connections) {
      if (!TryGetConnectedNodes(connection, nodeByPort, out var source, out var target)) {
        continue;
      }

      if (!incoming.TryGetValue(target, out var neighbors)) {
        neighbors = [];
        incoming[target] = neighbors;
      }

      neighbors.Add(source);
    }

    return incoming;
  }

  private static Dictionary<Guid, HashSet<Guid>> BuildUndirectedLookup(NetworkModel network, IReadOnlyDictionary<Guid, NodeModel> nodeByPort) {
    var adjacency = new Dictionary<Guid, HashSet<Guid>>();

    foreach (var connection in network.Connections) {
      if (!TryGetConnectedNodes(connection, nodeByPort, out var source, out var target)) {
        continue;
      }

      AddBidirectional(adjacency, source, target);
    }

    return adjacency;
  }

  private static bool TryGetConnectedNodes(
    ConnectionModel connection,
    IReadOnlyDictionary<Guid, NodeModel> nodeByPort,
    out Guid sourceNodeId,
    out Guid targetNodeId) {
    sourceNodeId = default;
    targetNodeId = default;

    if (!nodeByPort.TryGetValue(connection.OutputPortId, out var sourceNode)) {
      return false;
    }

    if (!nodeByPort.TryGetValue(connection.InputPortId, out var targetNode)) {
      return false;
    }

    sourceNodeId = sourceNode.Id;
    targetNodeId = targetNode.Id;
    return true;
  }

  private static bool CanReach(Guid sourceNodeId, Guid targetNodeId, IReadOnlyDictionary<Guid, HashSet<Guid>> outgoing) {
    if (sourceNodeId == targetNodeId) {
      return true;
    }

    var visited = new HashSet<Guid> { sourceNodeId };
    var queue = new Queue<Guid>();
    queue.Enqueue(sourceNodeId);

    while (queue.Count > 0) {
      var current = queue.Dequeue();
      if (!outgoing.TryGetValue(current, out var neighbors)) {
        continue;
      }

      foreach (var neighbor in neighbors) {
        if (!visited.Add(neighbor)) {
          continue;
        }

        if (neighbor == targetNodeId) {
          return true;
        }

        queue.Enqueue(neighbor);
      }
    }

    return false;
  }

  private static void AddBidirectional(Dictionary<Guid, HashSet<Guid>> adjacency, Guid left, Guid right) {
    if (!adjacency.TryGetValue(left, out var leftNeighbors)) {
      leftNeighbors = [];
      adjacency[left] = leftNeighbors;
    }

    leftNeighbors.Add(right);

    if (!adjacency.TryGetValue(right, out var rightNeighbors)) {
      rightNeighbors = [];
      adjacency[right] = rightNeighbors;
    }

    rightNeighbors.Add(left);
  }
}
