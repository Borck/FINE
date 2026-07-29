namespace FINE.Toolkit.NodeList;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

/// <summary>
/// A wrap panel that only realizes the item containers needed for the currently visible rows.
/// Assumes every item is the same size (measured from the first realized item), which holds for the
/// "Add node" palette where every tile uses the same scaled-down node preview template.
/// Used as the ItemsPanel for <see cref="NodeListView"/>'s tiles display mode so that having many
/// NodeTemplates no longer means constructing a full NodeView per template up front.
/// </summary>
public class VirtualizingWrapPanel : VirtualizingPanel, IScrollInfo {
  private const double LineSize = 32;

  private Size _extent;
  private Size _viewport;
  private Point _offset;
  private Size? _itemSize;
  private int _columns = 1;

  public VirtualizingWrapPanel() {
    ClipToBounds = true;
  }

  // Panel.ItemContainerGenerator is typed as the IItemContainerGenerator interface, which doesn't
  // expose IndexFromContainer - that's only on the concrete ItemContainerGenerator class.
  private ItemContainerGenerator Generator => (ItemContainerGenerator)ItemContainerGenerator;

  #region IScrollInfo
  public bool CanVerticallyScroll { get; set; }
  public bool CanHorizontallyScroll { get; set; }

  public double ExtentWidth => _extent.Width;
  public double ExtentHeight => _extent.Height;
  public double ViewportWidth => _viewport.Width;
  public double ViewportHeight => _viewport.Height;
  public double HorizontalOffset => _offset.X;
  public double VerticalOffset => _offset.Y;
  public ScrollViewer ScrollOwner { get; set; }

  public void LineUp() => SetVerticalOffset(VerticalOffset - LineSize);
  public void LineDown() => SetVerticalOffset(VerticalOffset + LineSize);
  public void LineLeft() { }
  public void LineRight() { }

  public void PageUp() => SetVerticalOffset(VerticalOffset - _viewport.Height);
  public void PageDown() => SetVerticalOffset(VerticalOffset + _viewport.Height);
  public void PageLeft() { }
  public void PageRight() { }

  public void MouseWheelUp() => SetVerticalOffset(VerticalOffset - LineSize * 3);
  public void MouseWheelDown() => SetVerticalOffset(VerticalOffset + LineSize * 3);
  public void MouseWheelLeft() { }
  public void MouseWheelRight() { }

  public void SetHorizontalOffset(double offset) {
    // Content always fills the available width, there is nothing to scroll horizontally.
  }

  public void SetVerticalOffset(double offset) {
    var clamped = Math.Max(0, Math.Min(offset, Math.Max(0, _extent.Height - _viewport.Height)));
    if (clamped == _offset.Y) {
      return;
    }

    _offset.Y = clamped;
    ScrollOwner?.InvalidateScrollInfo();
    InvalidateMeasure();
  }

  public Rect MakeVisible(Visual visual, Rect rectangle) {
    if (visual == null || _itemSize == null) {
      return rectangle;
    }

    var index = GetIndexFromContainer(visual);
    if (index < 0) {
      return rectangle;
    }

    var rowTop = index / _columns * _itemSize.Value.Height;
    var rowBottom = rowTop + _itemSize.Value.Height;

    if (rowTop < VerticalOffset) {
      SetVerticalOffset(rowTop);
    } else if (rowBottom > VerticalOffset + _viewport.Height) {
      SetVerticalOffset(rowBottom - _viewport.Height);
    }

    return rectangle;
  }
  #endregion

  private int GetIndexFromContainer(DependencyObject container) {
    while (container != null && !InternalChildren.Contains(container as UIElement)) {
      container = VisualTreeHelper.GetParent(container);
    }

    return container == null ? -1 : Generator.IndexFromContainer(container);
  }

  protected override Size MeasureOverride(Size availableSize) {
    // Accessing InternalChildren is what makes WPF associate this panel with its
    // ItemContainerGenerator; until it's touched once, ItemContainerGenerator returns null.
    _ = InternalChildren;

    var itemsControl = ItemsControl.GetItemsOwner(this);
    var itemCount = itemsControl?.HasItems == true ? itemsControl.Items.Count : 0;

    CanVerticallyScroll = true;
    CanHorizontallyScroll = false;

    if (itemCount == 0) {
      CleanUpItems(0, -1);
      _extent = new Size(availableSize.Width, 0);
      _viewport = availableSize;
      _offset = new Point(0, 0);
      ScrollOwner?.InvalidateScrollInfo();
      return new Size(availableSize.Width, 0);
    }

    EnsureItemSize(availableSize);
    var itemWidth = _itemSize?.Width ?? availableSize.Width;
    var itemHeight = _itemSize?.Height ?? 0;

    _columns = itemWidth > 0
        ? Math.Max(1, (int)Math.Floor(availableSize.Width / itemWidth))
        : 1;
    var rowCount = (int)Math.Ceiling(itemCount / (double)_columns);
    var extentHeight = rowCount * itemHeight;

    _viewport = availableSize;
    _extent = new Size(availableSize.Width, extentHeight);
    _offset.Y = Math.Max(0, Math.Min(_offset.Y, Math.Max(0, extentHeight - _viewport.Height)));

    var firstVisibleRow = itemHeight > 0 ? Math.Max(0, (int)Math.Floor(_offset.Y / itemHeight) - 1) : 0;
    var lastVisibleRow = itemHeight > 0
        ? Math.Min(rowCount - 1, (int)Math.Ceiling((_offset.Y + _viewport.Height) / itemHeight) + 1)
        : rowCount - 1;

    var firstVisibleIndex = Math.Min(itemCount - 1, firstVisibleRow * _columns);
    var lastVisibleIndex = Math.Min(itemCount - 1, (lastVisibleRow + 1) * _columns - 1);

    RealizeItems(firstVisibleIndex, lastVisibleIndex, new Size(itemWidth, itemHeight));
    CleanUpItems(firstVisibleIndex, lastVisibleIndex);

    ScrollOwner?.InvalidateScrollInfo();

    return new Size(availableSize.Width, Math.Min(availableSize.Height, extentHeight));
  }

  private void EnsureItemSize(Size availableSize) {
    if (_itemSize != null) {
      return;
    }

    var generatorStartPosition = ItemContainerGenerator.GeneratorPositionFromIndex(0);
    using (ItemContainerGenerator.StartAt(generatorStartPosition, GeneratorDirection.Forward, true)) {
      var container = (UIElement)ItemContainerGenerator.GenerateNext(out var isNewlyRealized);
      if (container == null) {
        return;
      }

      if (isNewlyRealized) {
        AddInternalChild(container);
        ItemContainerGenerator.PrepareItemContainer(container);
      }

      container.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
      _itemSize = container.DesiredSize;
    }
  }

  private void RealizeItems(int firstIndex, int lastIndex, Size itemSize) {
    if (firstIndex > lastIndex) {
      return;
    }

    var generatorStartPosition = ItemContainerGenerator.GeneratorPositionFromIndex(firstIndex);
    using (ItemContainerGenerator.StartAt(generatorStartPosition, GeneratorDirection.Forward, true)) {
      for (var i = firstIndex; i <= lastIndex; i++) {
        var container = (UIElement)ItemContainerGenerator.GenerateNext(out var isNewlyRealized);
        if (container == null) {
          break;
        }

        if (isNewlyRealized) {
          var insertIndex = GetInternalChildInsertIndex(i);
          InsertInternalChild(insertIndex, container);
          ItemContainerGenerator.PrepareItemContainer(container);
        }

        container.Measure(itemSize);
      }
    }
  }

  private int GetInternalChildInsertIndex(int itemIndex) {
    for (var childIndex = 0; childIndex < InternalChildren.Count; childIndex++) {
      var existingItemIndex = Generator.IndexFromContainer(InternalChildren[childIndex]);
      if (existingItemIndex > itemIndex) {
        return childIndex;
      }
    }

    return InternalChildren.Count;
  }

  private void CleanUpItems(int firstVisibleIndex, int lastVisibleIndex) {
    for (var childIndex = InternalChildren.Count - 1; childIndex >= 0; childIndex--) {
      var generatorPosition = new GeneratorPosition(childIndex, 0);
      var itemIndex = ItemContainerGenerator.IndexFromGeneratorPosition(generatorPosition);
      if (itemIndex < firstVisibleIndex || itemIndex > lastVisibleIndex) {
        ItemContainerGenerator.Remove(generatorPosition, 1);
        RemoveInternalChildRange(childIndex, 1);
      }
    }
  }

  protected override Size ArrangeOverride(Size finalSize) {
    if (_itemSize == null) {
      return finalSize;
    }

    var itemWidth = _itemSize.Value.Width;
    var itemHeight = _itemSize.Value.Height;

    foreach (UIElement child in InternalChildren) {
      var itemIndex = Generator.IndexFromContainer(child);
      if (itemIndex < 0) {
        continue;
      }

      var row = itemIndex / _columns;
      var col = itemIndex % _columns;
      var x = col * itemWidth;
      var y = row * itemHeight - _offset.Y;
      child.Arrange(new Rect(new Point(x, y), new Size(itemWidth, itemHeight)));
    }

    return finalSize;
  }

  protected override void BringIndexIntoView(int index) {
    if (_itemSize == null || _columns <= 0) {
      return;
    }

    var rowTop = index / _columns * _itemSize.Value.Height;
    var rowBottom = rowTop + _itemSize.Value.Height;

    if (rowTop < VerticalOffset) {
      SetVerticalOffset(rowTop);
    } else if (rowBottom > VerticalOffset + _viewport.Height) {
      SetVerticalOffset(rowBottom - _viewport.Height);
    }
  }
}
