namespace NodeNetwork.Toolkit.Blazor.ContextMenu {
  /// <summary>
  /// Search/filter model for context menu command lists.
  /// </summary>
  public class SearchableContextMenuModel {
    /// <summary>
    /// All available commands.
    /// </summary>
    public List<LabeledCommand> Commands { get; } = [];

    /// <summary>
    /// Active search filter.
    /// </summary>
    public string SearchQuery { get; set; } = string.Empty;

    /// <summary>
    /// Maximum number of displayed matching commands.
    /// </summary>
    public int MaxItemsDisplayed { get; set; } = int.MaxValue;

    /// <summary>
    /// Returns the currently visible commands based on visibility and search criteria.
    /// </summary>
    public IReadOnlyList<LabeledCommand> GetVisibleCommands() {
      var query = SearchQuery?.Trim() ?? string.Empty;

      var filtered = Commands.Where(static c => c.Visible);
      if (query.Length > 0) {
        filtered = filtered.Where(command => command.Label.Contains(query, StringComparison.OrdinalIgnoreCase));
      }

      return filtered.Take(Math.Max(0, MaxItemsDisplayed)).ToArray();
    }
  }
}
