namespace ReactiveUI.Legacy;

#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq.Expressions;
using ReactiveUI;

public static class ReactiveUiLegacyExtensions {
  public static IReactiveBinding<TView, IEnumerable<TItem>> BindList<TView, TViewModel, TItem>(
      this TView view,
      TViewModel? viewModel,
      Expression<System.Func<TViewModel, IEnumerable<TItem>>> vmProperty,
      Expression<System.Func<TView, IEnumerable<TItem>>> viewProperty)
      where TView : class, IViewFor<TViewModel>
      where TViewModel : class => view.OneWayBind(viewModel, vmProperty, viewProperty);
}
#endif
