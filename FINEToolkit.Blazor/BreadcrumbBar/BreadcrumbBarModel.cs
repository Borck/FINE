namespace FINE.Toolkit.Blazor.BreadcrumbBar;

/// <summary>
/// Represents a single breadcrumb item.
/// </summary>
public sealed class BreadcrumbItemModel {
  /// <summary>
  /// Display name.
  /// </summary>
  public required string Name { get; init; }

  /// <summary>
  /// Optional custom payload.
  /// </summary>
  public object? Tag { get; init; }
}

/// <summary>
/// Maintains breadcrumb path state and navigation operations.
/// </summary>
public sealed class BreadcrumbBarModel {
  /// <summary>
  /// Active breadcrumb path.
  /// </summary>
  public List<BreadcrumbItemModel> ActivePath { get; } = [];

  /// <summary>
  /// Deepest active crumb.
  /// </summary>
  public BreadcrumbItemModel? ActiveItem => ActivePath.Count == 0 ? null : ActivePath[^1];

  /// <summary>
  /// Appends a crumb to the active path.
  /// </summary>
  public void Push(BreadcrumbItemModel crumb) {
    ArgumentNullException.ThrowIfNull(crumb);
    ActivePath.Add(crumb);
  }

  /// <summary>
  /// Removes the deepest crumb if available.
  /// </summary>
  public BreadcrumbItemModel? Pop() {
    if (ActivePath.Count == 0) {
      return null;
    }

    var last = ActivePath[^1];
    ActivePath.RemoveAt(ActivePath.Count - 1);
    return last;
  }

  /// <summary>
  /// Selects a crumb and truncates all deeper crumbs.
  /// </summary>
  public void SelectCrumb(BreadcrumbItemModel crumb) {
    ArgumentNullException.ThrowIfNull(crumb);

    var index = ActivePath.IndexOf(crumb);
    if (index < 0) {
      return;
    }

    ActivePath.RemoveRange(index + 1, ActivePath.Count - (index + 1));
  }

  /// <summary>
  /// Clears the path.
  /// </summary>
  public void Clear() => ActivePath.Clear();
}
