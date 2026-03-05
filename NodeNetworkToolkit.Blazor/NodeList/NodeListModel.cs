namespace NodeNetwork.Toolkit.Blazor.NodeList {
  using NodeNetwork.Blazor.Models;

  /// <summary>
  /// Model for searchable node template lists.
  /// </summary>
  public sealed class NodeListModel {
    /// <summary>
    /// Rendering mode hint.
    /// </summary>
    public enum DisplayMode {
      Tiles,
      List
    }

    /// <summary>
    /// Display title for the list.
    /// </summary>
    public string Title { get; set; } = "Add node";

    /// <summary>
    /// Label to use when no items match the current query.
    /// </summary>
    public string EmptyLabel { get; set; } = "No matching nodes found.";

    /// <summary>
    /// Preferred display mode.
    /// </summary>
    public DisplayMode Display { get; set; } = DisplayMode.Tiles;

    /// <summary>
    /// Available node templates.
    /// </summary>
    public List<NodeTemplate> NodeTemplates { get; } = [];

    /// <summary>
    /// Search filter.
    /// </summary>
    public string SearchQuery { get; set; } = string.Empty;

    /// <summary>
    /// Returns template instances visible with the current filter.
    /// </summary>
    public IReadOnlyList<NodeModel> GetVisibleNodes() {
      var query = SearchQuery?.Trim() ?? string.Empty;

      if (query.Length == 0) {
        return NodeTemplates.Select(template => template.Instance).ToArray();
      }

      return NodeTemplates
        .Select(template => template.Instance)
        .Where(node => (node.Name ?? string.Empty).Contains(query, StringComparison.OrdinalIgnoreCase))
        .ToArray();
    }

    /// <summary>
    /// Adds a new template from a node factory.
    /// </summary>
    public void AddNodeType(Func<NodeModel> factory) {
      ArgumentNullException.ThrowIfNull(factory);
      NodeTemplates.Add(new NodeTemplate(factory));
    }
  }
}
