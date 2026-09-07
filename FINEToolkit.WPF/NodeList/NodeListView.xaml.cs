namespace FINE.Toolkit.NodeList;

using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DynamicData;
using FINE.ViewModels;
using ReactiveUI;

public partial class NodeListView : UserControl, IViewFor<NodeListViewModel> {
  #region ViewModel
  public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
    nameof(ViewModel),
    typeof(NodeListViewModel),
    typeof(NodeListView),
    new PropertyMetadata(null)
  );

  public NodeListViewModel ViewModel {
    get => (NodeListViewModel)GetValue(ViewModelProperty);
    set => SetValue(ViewModelProperty, value);
  }

  object IViewFor.ViewModel {
    get => ViewModel;
    set => ViewModel = (NodeListViewModel)value;
  }
  #endregion

  #region Show/Hide properties
  public static readonly DependencyProperty ShowSearchProperty =
      DependencyProperty.Register(nameof(ShowSearch), typeof(bool), typeof(NodeListView), new PropertyMetadata(true));
  public static readonly DependencyProperty ShowDisplayModeSelectorProperty =
      DependencyProperty.Register(nameof(ShowDisplayModeSelector), typeof(bool), typeof(NodeListView), new PropertyMetadata(true));
  public static readonly DependencyProperty ShowTitleProperty =
      DependencyProperty.Register(nameof(ShowTitle), typeof(bool), typeof(NodeListView), new PropertyMetadata(true));

  public bool ShowSearch {
    get => (bool)GetValue(ShowSearchProperty);
    set => SetValue(ShowSearchProperty, value);
  }

  public bool ShowDisplayModeSelector {
    get => (bool)GetValue(ShowDisplayModeSelectorProperty);
    set => SetValue(ShowDisplayModeSelectorProperty, value);
  }

  public bool ShowTitle {
    get => (bool)GetValue(ShowTitleProperty);
    set => SetValue(ShowTitleProperty, value);
  }
  #endregion

  #region Colors
  public static readonly DependencyProperty ListEntryBackgroundBrushProperty =
      DependencyProperty.Register(nameof(ListEntryBackgroundBrush), typeof(Brush), typeof(NodeListView), new PropertyMetadata(new SolidColorBrush(Colors.White)));

  public Brush ListEntryBackgroundBrush {
    get => (Brush)GetValue(ListEntryBackgroundBrushProperty);
    set => SetValue(ListEntryBackgroundBrushProperty, value);
  }

  public static readonly DependencyProperty ListEntryBackgroundMouseOverBrushProperty =
      DependencyProperty.Register(nameof(ListEntryBackgroundMouseOverBrush), typeof(Brush), typeof(NodeListView), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0xf7, 0xf7, 0xf7))));

  public Brush ListEntryBackgroundMouseOverBrush {
    get => (Brush)GetValue(ListEntryBackgroundMouseOverBrushProperty);
    set => SetValue(ListEntryBackgroundMouseOverBrushProperty, value);
  }

  public static readonly DependencyProperty ListEntryHandleBrushProperty =
      DependencyProperty.Register(nameof(ListEntryHandleBrush), typeof(Brush), typeof(NodeListView), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99))));

  public Brush ListEntryHandleBrush {
    get => (Brush)GetValue(ListEntryHandleBrushProperty);
    set => SetValue(ListEntryHandleBrushProperty, value);
  }
  #endregion

  public CollectionViewSource CVS { get; } = new CollectionViewSource();

  public NodeListView() {
    InitializeComponent();
    if (DesignerProperties.GetIsInDesignMode(this)) { return; }

    viewComboBox.ItemsSource = Enum.GetValues(typeof(NodeListViewModel.DisplayMode)).Cast<NodeListViewModel.DisplayMode>();

    this.WhenActivated(d => {
      this.Bind(ViewModel, vm => vm.Display, v => v.viewComboBox.SelectedItem).DisposeWith(d);

      // Dynamisches ItemTemplate umschalten
      this.OneWayBind(ViewModel, vm => vm.Display, v => v.elementsList.ItemTemplate,
          displayMode => displayMode == NodeListViewModel.DisplayMode.Tiles
              ? (DataTemplate)Resources["tilesTemplate"]
              : (DataTemplate)Resources["listTemplate"])
          .DisposeWith(d);

      // Dynamisches ItemsPanel (VirtualizingWrapPanel vs. VirtualizingStackPanel)
      this.OneWayBind(ViewModel, vm => vm.Display, v => v.elementsList.ItemsPanel,
          displayMode => displayMode == NodeListViewModel.DisplayMode.Tiles
              ? (ItemsPanelTemplate)Resources["tilesItemsPanelTemplate"]
              : (ItemsPanelTemplate)Resources["listItemsPanelTemplate"])
          .DisposeWith(d);

      this.Bind(ViewModel, vm => vm.SearchQuery, v => v.searchBox.Text).DisposeWith(d);

      // Knoten reaktiv auf den MainThread binden
      this.WhenAnyValue(v => v.ViewModel.VisibleNodes)
          .Where(nodes => nodes != null)
          .Select(nodes => nodes.Connect())
          .Switch()
          .ObserveOn(RxApp.MainThreadScheduler)
          .Bind(out var bindableList)
          .Subscribe()
          .DisposeWith(d);

      CVS.Source = bindableList;
      elementsList.ItemsSource = CVS.View;

      this.WhenAnyObservable(v => v.ViewModel.VisibleNodes.CountChanged)
          .ObserveOn(RxApp.MainThreadScheduler)
          .Select(count => count == 0 ? Visibility.Visible : Visibility.Collapsed)
          .BindTo(this, v => v.emptyMessage.Visibility)
          .DisposeWith(d);

      this.OneWayBind(ViewModel, vm => vm.Title, v => v.titleLabel.Text).DisposeWith(d);
      this.OneWayBind(ViewModel, vm => vm.EmptyLabel, v => v.emptyMessage.Text).DisposeWith(d);

      this.WhenAnyValue(v => v.searchBox.IsFocused, v => v.searchBox.Text)
          .Select(t => !t.Item1 && string.IsNullOrWhiteSpace(t.Item2) ? Visibility.Visible : Visibility.Collapsed)
          .BindTo(this, v => v.emptySearchBoxMessage.Visibility)
          .DisposeWith(d);

      this.WhenAnyValue(v => v.ShowSearch)
          .Select(show => show ? Visibility.Visible : Visibility.Collapsed)
          .BindTo(this, v => v.searchBoxGrid.Visibility)
          .DisposeWith(d);

      this.WhenAnyValue(v => v.ShowDisplayModeSelector)
          .Select(show => show ? Visibility.Visible : Visibility.Collapsed)
          .BindTo(this, v => v.viewComboBox.Visibility)
          .DisposeWith(d);

      this.WhenAnyValue(v => v.ShowTitle)
          .Select(show => show ? Visibility.Visible : Visibility.Collapsed)
          .BindTo(this, v => v.titleLabel.Visibility)
          .DisposeWith(d);
    });
  }

  private void OnNodeMouseMove(object sender, MouseEventArgs e) {
    if (e.LeftButton == MouseButtonState.Pressed) {
      if ((sender as FrameworkElement)?.DataContext is not NodeViewModel nodeVM || ViewModel?.NodeTemplates == null) {
        return;
      }

      var template = ViewModel.NodeTemplates.Items.FirstOrDefault(t => t.Instance == nodeVM);
      if (template?.Factory != null) {
        var newNodeVM = template.Factory();
        DragDrop.DoDragDrop(this, new DataObject("nodeVM", newNodeVM), DragDropEffects.Copy);
      }
    }
  }
}
