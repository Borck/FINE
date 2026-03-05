namespace FINE.Toolkit.Blazor.ContextMenu {
  using System.Globalization;
  using FINE.Blazor.Models;
  using FINE.Blazor.Validation;

  /// <summary>
  /// Toolkit model for adding nodes from templates to a network.
  /// </summary>
  public sealed class AddNodeContextMenuModel : SearchableContextMenuModel {
    /// <summary>
    /// The target network where nodes are added.
    /// </summary>
    public required NetworkModel Network { get; init; }

    /// <summary>
    /// Label format used for generated add-node entries.
    /// </summary>
    public string LabelFormat { get; init; } = "{0}";

    /// <summary>
    /// Function used to determine initial node placement.
    /// </summary>
    public Func<NodeModel, CanvasPoint> NodePositionFunc { get; set; } = static _ => default;

    /// <summary>
    /// Callback invoked after a node was added.
    /// </summary>
    public Action<NodeModel> OnNodeAdded { get; set; } = static _ => { };

    /// <summary>
    /// Adds a new node template command.
    /// </summary>
    public void AddNodeType(NodeTemplate template) {
      ArgumentNullException.ThrowIfNull(template);

      Commands.Add(new LabeledCommand {
        Label = string.Format(CultureInfo.CurrentCulture, LabelFormat, template.Instance.Name),
        CommandParameter = template,
        Command = parameter => {
          var nodeTemplate = (NodeTemplate)parameter!;
          var node = nodeTemplate.Factory();
          node.Position = NodePositionFunc(node);
          Network.Nodes.Add(node);
          OnNodeAdded(node);
          return Task.CompletedTask;
        }
      });
    }

    /// <summary>
    /// Adds a set of node templates.
    /// </summary>
    public void AddNodeTypes(IEnumerable<NodeTemplate> templates) {
      ArgumentNullException.ThrowIfNull(templates);
      foreach (var template in templates) {
        AddNodeType(template);
      }
    }

    /// <summary>
    /// Returns templates that contain at least one endpoint connectable to the pending connection.
    /// </summary>
    public static IEnumerable<NodeTemplate> GetConnectableNodes(
      IEnumerable<NodeTemplate> candidates,
      NetworkModel network,
      PendingConnectionModel pendingConnection,
      INetworkConnectionValidator? validator = null) {
      ArgumentNullException.ThrowIfNull(candidates);
      ArgumentNullException.ThrowIfNull(network);

      foreach (var template in candidates) {
        if (pendingConnection.InputIsLocked) {
          if (GetConnectableOutputs(template.Instance, network, pendingConnection, validator).Any()) {
            yield return template;
          }
        } else {
          if (GetConnectableInputs(template.Instance, network, pendingConnection, validator).Any()) {
            yield return template;
          }
        }
      }
    }

    /// <summary>
    /// Returns output ports that can connect to a locked pending input.
    /// </summary>
    public static IEnumerable<NodePortModel> GetConnectableOutputs(
      NodeModel node,
      NetworkModel network,
      PendingConnectionModel pendingConnection,
      INetworkConnectionValidator? validator = null) {
      ArgumentNullException.ThrowIfNull(node);
      ArgumentNullException.ThrowIfNull(network);

      if (!pendingConnection.InputIsLocked || pendingConnection.InputPortId is null) {
        yield break;
      }

      var input = FindPort(network, pendingConnection.InputPortId.Value);
      var resolvedValidator = validator ?? new DefaultNetworkConnectionValidator();

      foreach (var output in node.Outputs) {
        if (resolvedValidator.Validate(network, output, input).IsAllowed) {
          yield return output;
        }
      }
    }

    /// <summary>
    /// Returns input ports that can connect to a locked pending output.
    /// </summary>
    public static IEnumerable<NodePortModel> GetConnectableInputs(
      NodeModel node,
      NetworkModel network,
      PendingConnectionModel pendingConnection,
      INetworkConnectionValidator? validator = null) {
      ArgumentNullException.ThrowIfNull(node);
      ArgumentNullException.ThrowIfNull(network);

      if (!pendingConnection.OutputIsLocked || pendingConnection.OutputPortId is null) {
        yield break;
      }

      var output = FindPort(network, pendingConnection.OutputPortId.Value);
      var resolvedValidator = validator ?? new DefaultNetworkConnectionValidator();

      foreach (var input in node.Inputs) {
        if (resolvedValidator.Validate(network, output, input).IsAllowed) {
          yield return input;
        }
      }
    }

    private static NodePortModel FindPort(NetworkModel network, Guid id) =>
      network.Nodes.SelectMany(n => n.GetPorts()).First(port => port.Id == id);
  }
}
