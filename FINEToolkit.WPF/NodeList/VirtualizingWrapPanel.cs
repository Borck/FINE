namespace FINE.Toolkit.NodeList;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
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

  // Recycle() is only exposed via IRecyclingItemContainerGenerator, which ItemContainerGenerator
  // implements explicitly (not as a regular public member).
  private IRecyclingItemContainerGenerator RecyclingGenerator => (IRecyclingItemContainerGenerator)ItemContainerGenerator;

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

  // ItemsControl.GetItemsOwner(this).Items.Count is only correct for a top-level (ungrouped) panel.
  // When this panel is the ItemsHost inside a GroupItem's ItemsPresenter (grouping active), GetItemsOwner
  // still resolves to the top-level ItemsControl, whose Items is the full flat collection across every
  // group - not just this group's items. Use the GroupItem's own CollectionViewGroup.ItemCount instead
  // when we're scoped to a group, since that's what this panel instance actually needs to lay out.
  private int GetItemCount() {
    // This panel's TemplatedParent is the ItemsPresenter that hosts it; that ItemsPresenter's own
    // TemplatedParent is whatever control's ControlTemplate declared it - a GroupItem when this
    // panel is scoped to one group (grouping active), or the top-level ItemsControl otherwise.
    if (TemplatedParent is FrameworkElement itemsPresenter &&
        itemsPresenter.TemplatedParent is GroupItem { Content: CollectionViewGroup group }) {
      return group.ItemCount;
    }

    var itemsControl = ItemsControl.GetItemsOwner(this);
    return itemsControl?.HasItems == true ? itemsControl.Items.Count : 0;
  }

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

    var itemCount = GetItemCount();

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

    RefreshItemSize();
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

  // Re-derives the uniform tile size from a reference container on every measure pass rather than
  // caching it once. A tile's real content only appears once its ViewModelViewHost activates (see
  // the container-recycling note on CleanUpItems below) - if we locked in whatever (possibly
  // degenerate) size we saw before that happened, tiles would never recover once content arrived.
  // Re-measuring is effectively free once the container's measure is valid and the constraint is
  // unchanged (WPF short-circuits to the cached DesiredSize), so this only does real work when
  // something actually invalidated the container.
  //
  // Measured with PositiveInfinity, matching how NodeView is actually designed to be sized: in its
  // real usage (a node placed in NetworkView's Canvas), Canvas always measures children with
  // infinite available space and NodeView relies on that plus its own MinWidth/MinHeight - so this
  // mirrors the constraint NodeView is meant to compute its natural size under.
  private void RefreshItemSize() {
    UIElement referenceContainer = InternalChildren.Count > 0 ? InternalChildren[0] : null;

    if (referenceContainer == null) {
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

        referenceContainer = container;
      }
    }

    referenceContainer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
    var measured = referenceContainer.DesiredSize;
    if (measured.Width > 0 && measured.Height > 0) {
      _itemSize = measured;
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

  // Recycles rather than destroys containers that scroll out of view. Each tile's content is a
  // ReactiveUI ViewModelViewHost, which only resolves and sets its View once its WhenActivated block
  // fires - and that's gated on the container's WPF Loaded event. Loaded fires (at the earliest) on
  // the next layout pass after a container is connected to the tree, never synchronously within the
  // same Measure/Arrange call that created it. Generator.Remove() fully destroys the container, so
  // any tile whose row/column changed (e.g. every resize, since that reshuffles which items are
  // realized) got torn down and rebuilt from scratch - forcing it through Loaded/WhenActivated again
  // before anything would render, and if it got torn down again before that finished, it never did.
  // Recycle() keeps the container's .NET instance (and its already-activated ViewModelViewHost)
  // alive in the generator's pool for reuse by a different item, sidestepping Loaded entirely for
  // every reuse after the first.
  private void CleanUpItems(int firstVisibleIndex, int lastVisibleIndex) {
    for (var childIndex = InternalChildren.Count - 1; childIndex >= 0; childIndex--) {
      var generatorPosition = new GeneratorPosition(childIndex, 0);
      var itemIndex = ItemContainerGenerator.IndexFromGeneratorPosition(generatorPosition);
      if (itemIndex < firstVisibleIndex || itemIndex > lastVisibleIndex) {
        RecyclingGenerator.Recycle(generatorPosition, 1);
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
