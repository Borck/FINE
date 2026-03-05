namespace FINE.Toolkit.Blazor.Layout.ForceDirected;

using FINE.Blazor.Models;

/// <summary>
/// Repositions nodes using a force-directed simulation model.
/// </summary>
public sealed class ForceDirectedLayouter {
  /// <summary>
  /// Applies force-directed layout directly to node positions in the given network.
  /// </summary>
  /// <param name="network">The network to layout.</param>
  /// <param name="configuration">Optional simulation settings.</param>
  public void Layout(NetworkModel network, ForceDirectedLayoutConfiguration? configuration = null) {
    ArgumentNullException.ThrowIfNull(network);

    var config = configuration ?? new ForceDirectedLayoutConfiguration();
    ValidateConfiguration(config);

    if (network.Nodes.Count <= 1) {
      return;
    }

    var nodes = network.Nodes;
    var nodeIndex = nodes.Select((node, index) => (node.Id, index)).ToDictionary(pair => pair.Id, pair => pair.index);

    var positions = nodes.Select(static node => node.Position).ToArray();
    var velocities = new (double X, double Y)[nodes.Count];

    ApplyInitialJitter(positions, config.InitialJitter);

    var edges = BuildEdges(network, nodeIndex);
    var forces = new (double X, double Y)[nodes.Count];

    for (var iteration = 0; iteration < config.Iterations; iteration++) {
      Array.Clear(forces);

      ApplyRepulsion(positions, forces, config.RepulsionStrength);
      ApplySprings(positions, forces, edges, config.SpringStrength, config.SpringLength);
      ApplyCenterAttraction(positions, forces, config.CenterAttraction);
      ApplyVelocities(positions, velocities, forces, config.Damping, config.MaxStep);
    }

    for (var i = 0; i < nodes.Count; i++) {
      nodes[i].Position = positions[i];
    }
  }

  /// <summary>
  /// Compatibility API equivalent of FINEToolkit.WPF's synchronous force-directed layout.
  /// </summary>
  public void Layout(Configuration configuration, int maxIterations) {
    ArgumentNullException.ThrowIfNull(configuration);
    ArgumentOutOfRangeException.ThrowIfNegative(maxIterations);

    var network = configuration.Network;
    if (network.Nodes.Count <= 1) {
      return;
    }

    ApplyRandomShift(network);

    var elapsedMsPerIteration = (int)Math.Ceiling(10d / Math.Max(1, configuration.UpdatesPerIteration));
    var state = new CompatibilityState(network.Nodes.Count);
    for (var i = 0; i < maxIterations * Math.Max(1, configuration.UpdatesPerIteration); i++) {
      StepCompatibilitySimulation(configuration, state, elapsedMsPerIteration);
    }
  }

  /// <summary>
  /// Compatibility API equivalent of FINEToolkit.WPF's asynchronous layout simulation.
  /// </summary>
  public async Task LayoutAsync(Configuration configuration, CancellationToken token) {
    ArgumentNullException.ThrowIfNull(configuration);

    var network = configuration.Network;
    if (network.Nodes.Count <= 1) {
      return;
    }

    ApplyRandomShift(network);

    var start = DateTime.UtcNow;
    var previous = TimeSpan.Zero;
    var state = new CompatibilityState(network.Nodes.Count);

    while (!token.IsCancellationRequested) {
      var current = DateTime.UtcNow - start;
      var delta = current - previous;
      previous = current;

      var virtualDelta = (int)(delta.TotalMilliseconds * configuration.TimeModifier);
      var updates = Math.Max(1, configuration.UpdatesPerIteration);
      var deltaPerUpdate = Math.Max(1, virtualDelta / updates);

      for (var i = 0; i < updates; i++) {
        StepCompatibilitySimulation(configuration, state, deltaPerUpdate);
      }

      await Task.Delay(14, token).ConfigureAwait(false);
    }
  }

  private static void StepCompatibilitySimulation(Configuration config, CompatibilityState state, int deltaTMillis) {
    var network = config.Network;
    var nodeCount = network.Nodes.Count;
    var nodeIndex = network.Nodes.Select((node, index) => (node, index)).ToDictionary(x => x.node.Id, x => x.index);

    var forces = new (double X, double Y)[nodeCount];

    var portOwner = new Dictionary<Guid, int>();
    foreach (var node in network.Nodes) {
      var owner = nodeIndex[node.Id];
      foreach (var port in node.GetPorts()) {
        portOwner[port.Id] = owner;
      }
    }

    for (var i = 0; i < nodeCount; i++) {
      var node = network.Nodes[i];
      if (config.IsFixedNode(node)) {
        continue;
      }

      var total = (X: 0d, Y: 0d);

      foreach (var connection in network.Connections) {
        if (!portOwner.TryGetValue(connection.OutputPortId, out var sourceIndex) ||
            !portOwner.TryGetValue(connection.InputPortId, out var targetIndex)) {
          continue;
        }

        var isSource = i == sourceIndex;
        var isTarget = i == targetIndex;
        if (!isSource && !isTarget) {
          continue;
        }

        var sourceNode = network.Nodes[sourceIndex];
        var targetNode = network.Nodes[targetIndex];
        var sourcePos = new CanvasPoint(sourceNode.Position.X + sourceNode.Size.Width, sourceNode.Position.Y + sourceNode.Size.Height / 2d);
        var targetPos = new CanvasPoint(targetNode.Position.X, targetNode.Position.Y + targetNode.Size.Height / 2d);

        var dx = targetPos.X - sourcePos.X;
        var dy = targetPos.Y - sourcePos.Y;
        var distSq = Math.Max(1d, dx * dx + dy * dy);
        var dist = Math.Sqrt(distSq);

        var hook = (dist - config.EquilibriumDistance(connection)) * config.SpringConstant(connection);
        var dirX = dx / dist;
        var dirY = dy / dist;

        var springX = dirX * hook;
        var springY = dirY * hook;

        var row = config.RowForce(connection);
        var rowX = isSource ? row : -row;

        if (isSource) {
          total.X += springX + rowX;
          total.Y += springY;
        } else {
          total.X -= springX + rowX;
          total.Y -= springY;
        }
      }

      var centerX = node.Position.X + node.Size.Width / 2d;
      var centerY = node.Position.Y + node.Size.Height / 2d;
      for (var j = 0; j < nodeCount; j++) {
        if (i == j) {
          continue;
        }

        var other = network.Nodes[j];
        var otherCenterX = other.Position.X + other.Size.Width / 2d;
        var otherCenterY = other.Position.Y + other.Size.Height / 2d;

        var dx = otherCenterX - centerX;
        var dy = otherCenterY - centerY;
        var distSq = Math.Max(1d, dx * dx + dy * dy);
        var dist = Math.Sqrt(distSq);
        var nx = dx / dist;
        var ny = dy / dist;

        var repulsionX = nx * (-1d * ((node.Size.Width + other.Size.Width) / 2d) / dist);
        var repulsionY = ny * (-1d * ((node.Size.Height + other.Size.Height) / 2d) / dist);
        total.X += repulsionX * config.NodeRepulsionForce;
        total.Y += repulsionY * config.NodeRepulsionForce;
      }

      var gravity = 9.8f;
      var normalForce = gravity * config.NodeMass(node);
      var kineticFriction = normalForce * config.FrictionCoefficient(node);

      var (X, Y) = state.Speeds[i];
      var speedLength = Math.Sqrt(X * X + Y * Y);
      if (speedLength > 0d) {
        total.X += -kineticFriction * (X / speedLength);
        total.Y += -kineticFriction * (Y / speedLength);
      }

      forces[i] = total;
    }

    var deltaT = deltaTMillis / 1000d;
    for (var i = 0; i < nodeCount; i++) {
      var node = network.Nodes[i];
      if (config.IsFixedNode(node)) {
        continue;
      }

      var (X, Y) = state.Speeds[i];
      var force = forces[i];
      var mass = Math.Max(0.0001d, config.NodeMass(node));

      var newX = node.Position.X + X * deltaT + force.X * deltaT * deltaT / 2d;
      var newY = node.Position.Y + Y * deltaT + force.Y * deltaT * deltaT / 2d;

      state.Speeds[i] = (X + force.X / mass * deltaT, Y + force.Y / mass * deltaT);
      node.Position = new CanvasPoint(newX, newY);
    }
  }

  private static void ApplyRandomShift(NetworkModel network) {
    var random = new Random();
    foreach (var node in network.Nodes) {
      node.Position = new CanvasPoint(node.Position.X + random.NextDouble(), node.Position.Y + random.NextDouble());
    }
  }

  private static void ValidateConfiguration(ForceDirectedLayoutConfiguration configuration) {
    if (configuration.Iterations < 0) {
      throw new ArgumentOutOfRangeException(nameof(configuration.Iterations));
    }

    if (configuration.Damping is < 0d or > 1d) {
      throw new ArgumentOutOfRangeException(nameof(configuration.Damping));
    }

    if (configuration.SpringLength <= 0d) {
      throw new ArgumentOutOfRangeException(nameof(configuration.SpringLength));
    }

    if (configuration.MaxStep <= 0d) {
      throw new ArgumentOutOfRangeException(nameof(configuration.MaxStep));
    }

    if (configuration.InitialJitter < 0d) {
      throw new ArgumentOutOfRangeException(nameof(configuration.InitialJitter));
    }
  }

  private static List<(int Source, int Target)> BuildEdges(NetworkModel network, IReadOnlyDictionary<Guid, int> nodeIndex) {
    var portOwner = new Dictionary<Guid, int>();
    foreach (var node in network.Nodes) {
      var owner = nodeIndex[node.Id];
      foreach (var port in node.GetPorts()) {
        portOwner[port.Id] = owner;
      }
    }

    var edges = new List<(int Source, int Target)>(network.Connections.Count);
    foreach (var connection in network.Connections) {
      if (!portOwner.TryGetValue(connection.OutputPortId, out var source)) {
        continue;
      }

      if (!portOwner.TryGetValue(connection.InputPortId, out var target)) {
        continue;
      }

      if (source == target) {
        continue;
      }

      edges.Add((source, target));
    }

    return edges;
  }

  private static void ApplyInitialJitter(IList<CanvasPoint> positions, double jitter) {
    if (jitter <= 0d) {
      return;
    }

    var occupancy = new Dictionary<(long X, long Y), int>();
    for (var i = 0; i < positions.Count; i++) {
      var position = positions[i];
      var key = (BitConverter.DoubleToInt64Bits(position.X), BitConverter.DoubleToInt64Bits(position.Y));

      if (!occupancy.TryGetValue(key, out var count)) {
        occupancy[key] = 1;
        continue;
      }

      count++;
      occupancy[key] = count;

      var offset = jitter * count;
      var phase = 2.399963229728653d * i;
      positions[i] = new CanvasPoint(
          position.X + Math.Cos(phase) * offset,
          position.Y + Math.Sin(phase) * offset);
    }
  }

  private static void ApplyRepulsion(IReadOnlyList<CanvasPoint> positions, IList<(double X, double Y)> forces, double repulsionStrength) {
    for (var i = 0; i < positions.Count; i++) {
      for (var j = i + 1; j < positions.Count; j++) {
        var dx = positions[j].X - positions[i].X;
        var dy = positions[j].Y - positions[i].Y;
        var distanceSquared = dx * dx + dy * dy;

        if (distanceSquared < 1d) {
          distanceSquared = 1d;
        }

        var distance = Math.Sqrt(distanceSquared);
        var magnitude = repulsionStrength / distanceSquared;
        var fx = magnitude * (dx / distance);
        var fy = magnitude * (dy / distance);

        forces[i] = (forces[i].X - fx, forces[i].Y - fy);
        forces[j] = (forces[j].X + fx, forces[j].Y + fy);
      }
    }
  }

  private static void ApplySprings(
      IReadOnlyList<CanvasPoint> positions,
      IList<(double X, double Y)> forces,
      IReadOnlyList<(int Source, int Target)> edges,
      double springStrength,
      double springLength) {
    foreach (var (Source, Target) in edges) {
      var source = Source;
      var target = Target;

      var dx = positions[target].X - positions[source].X;
      var dy = positions[target].Y - positions[source].Y;
      var distanceSquared = dx * dx + dy * dy;

      if (distanceSquared < 1d) {
        distanceSquared = 1d;
      }

      var distance = Math.Sqrt(distanceSquared);
      var stretch = distance - springLength;
      var magnitude = springStrength * stretch;
      var fx = magnitude * (dx / distance);
      var fy = magnitude * (dy / distance);

      forces[source] = (forces[source].X + fx, forces[source].Y + fy);
      forces[target] = (forces[target].X - fx, forces[target].Y - fy);
    }
  }

  private static void ApplyCenterAttraction(IReadOnlyList<CanvasPoint> positions, IList<(double X, double Y)> forces, double centerAttraction) {
    for (var i = 0; i < positions.Count; i++) {
      forces[i] =
      (
          forces[i].X - positions[i].X * centerAttraction,
          forces[i].Y - positions[i].Y * centerAttraction
      );
    }
  }

  private static void ApplyVelocities(
      IList<CanvasPoint> positions,
      IList<(double X, double Y)> velocities,
      IReadOnlyList<(double X, double Y)> forces,
      double damping,
      double maxStep) {
    for (var i = 0; i < positions.Count; i++) {
      var vx = (velocities[i].X + forces[i].X) * damping;
      var vy = (velocities[i].Y + forces[i].Y) * damping;

      var speed = Math.Sqrt(vx * vx + vy * vy);
      if (speed > maxStep) {
        var scale = maxStep / speed;
        vx *= scale;
        vy *= scale;
      }

      velocities[i] = (vx, vy);
      positions[i] = new CanvasPoint(positions[i].X + vx, positions[i].Y + vy);
    }
  }

  private sealed class CompatibilityState(int nodeCount) {
    public (double X, double Y)[] Speeds { get; } = new (double X, double Y)[nodeCount];
  }
}

