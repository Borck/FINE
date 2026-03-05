namespace FINE.Toolkit.ContextMenu;

using System;
using System.Collections;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DynamicData;
using FINE.Utilities;
using ReactiveUI;

public partial class SearchableContextMenuView : IViewFor<SearchableContextMenuViewModel> {
  #region ViewModel
  public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
      typeof(SearchableContextMenuViewModel), typeof(SearchableContextMenuView), new PropertyMetadata(null));

  public SearchableContextMenuViewModel ViewModel {
    get => (SearchableContextMenuViewModel)GetValue(ViewModelProperty);
    set => SetValue(ViewModelProperty, value);
  }

  object IViewFor.ViewModel {
    get => ViewModel;
    set => ViewModel = (SearchableContextMenuViewModel)value;
  }
  #endregion

  #region ChildrenBelowSearch
  public static readonly DependencyProperty ChildrenBelowSearchProperty =
      DependencyProperty.Register(nameof(ChildrenBelowSearch), typeof(IEnumerable), typeof(SearchableContextMenuView), new PropertyMetadata(Array.Empty<object>()));

  public IEnumerable ChildrenBelowSearch {
    get => (IEnumerable)GetValue(ChildrenBelowSearchProperty);
    set => SetValue(ChildrenBelowSearchProperty, value);
  }
  #endregion

  #region ReferencePointElement
  public static readonly DependencyProperty ReferencePointElementProperty =
      DependencyProperty.Register(nameof(ReferencePointElement), typeof(IInputElement), typeof(SearchableContextMenuView), new PropertyMetadata(null));

  public IInputElement ReferencePointElement {
    get => (IInputElement)GetValue(ReferencePointElementProperty);
    set => SetValue(ReferencePointElementProperty, value);
  }
  #endregion

  #region OpenPoint
  public static readonly DependencyProperty OpenPointProperty =
      DependencyProperty.Register(nameof(OpenPoint), typeof(Point), typeof(SearchableContextMenuView), new PropertyMetadata(new Point()));

  public Point OpenPoint {
    get => (Point)GetValue(OpenPointProperty);
    private set => SetValue(OpenPointProperty, value);
  }
  #endregion

  public SearchableContextMenuView() {
    InitializeComponent();

    this.Bind(ViewModel, vm => vm.SearchQuery, v => v.SearchTextBox.Text);
    this.BindList(ViewModel, vm => vm.VisibleCommands, v => v.CollectionContainer.Collection);

    var myBinding = new Binding(nameof(ChildrenBelowSearch)) { Source = this };
    BindingOperations.SetBinding(ContainerBelowSearch, CollectionContainer.CollectionProperty, myBinding);

    Opened += (sender, args) => {
      SearchTextBox.Focus();
      if (ReferencePointElement != null) {
        OpenPoint = Mouse.GetPosition(ReferencePointElement);
      }
    };

    // This var is needed to ensure both key down and key up of arrow keys happened in the textbox,
    // otherwise moving into the textbox will immediately move out again.
    var arrowWasPressedInTextBox = false;

    SearchTextBox.PreviewKeyDown += (sender, args) => {
      if (args.Key is Key.Enter or Key.Return) {
        if (ViewModel.VisibleCommands.Count > 0) {
          var firstEntry = ViewModel.VisibleCommands.Items[0];
          firstEntry.Command.Execute(firstEntry.CommandParameter);
          IsOpen = false;
        }
      } else if (args.Key == Key.Escape && SearchTextBox.Text.Length > 0) {
        SearchTextBox.Text = "";
        args.Handled = true;
      } else if (args.Key is Key.Up or Key.Down) {
        arrowWasPressedInTextBox = true;
      }
    };
    SearchTextBox.PreviewKeyUp += (sender, args) => {
      if (arrowWasPressedInTextBox && args.Key is Key.Up or Key.Down) {
        arrowWasPressedInTextBox = false;

        var dir = args.Key == Key.Up ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next;
        var traversalRequest = new TraversalRequest(dir);
        var focusedElem = Keyboard.FocusedElement as FrameworkElement;
        focusedElem?.MoveFocus(traversalRequest);
      }
    };
    SearchMenuItem.GotKeyboardFocus += (sender, args) => SearchTextBox.Focus();
  }
}
