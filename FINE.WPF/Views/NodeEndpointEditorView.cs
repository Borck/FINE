namespace FINE.Views;

using System.Windows;
using System.Windows.Controls;
using FINE.ViewModels;
using ReactiveUI;

public class NodeEndpointEditorView : Control, IViewFor<NodeEndpointEditorViewModel> {
  #region ViewModel
  public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
      typeof(NodeEndpointEditorViewModel), typeof(NodeEndpointEditorView), new PropertyMetadata(null));

  public NodeEndpointEditorViewModel ViewModel {
    get => (NodeEndpointEditorViewModel)GetValue(ViewModelProperty);
    set => SetValue(ViewModelProperty, value);
  }

  object IViewFor.ViewModel {
    get => ViewModel;
    set => ViewModel = (NodeEndpointEditorViewModel)value;
  }
  #endregion

  public NodeEndpointEditorView() {
    DefaultStyleKey = typeof(NodeEndpointEditorView);
  }
}
