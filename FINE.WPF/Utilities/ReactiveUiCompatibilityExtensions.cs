namespace ReactiveUI;

using System;
using System.Collections.Generic;


public static class ReactiveUiCompatibilityExtensions {
  // Accepts ICollection<IDisposable> rather than the concrete CompositeDisposable so it also
  // works with ReactiveUI v24's WhenActivated(Action<MultipleDisposable>) activation blocks.
  public static T DisposeWith<T>(this T disposable, ICollection<IDisposable> disposables)
      where T : IDisposable {
    disposables.Add(disposable);
    return disposable;
  }
}
