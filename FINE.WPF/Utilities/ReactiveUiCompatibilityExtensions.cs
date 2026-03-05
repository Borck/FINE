namespace ReactiveUI;

using System;
using System.Reactive.Disposables;


public static class ReactiveUiCompatibilityExtensions {
  public static T DisposeWith<T>(this T disposable, CompositeDisposable compositeDisposable)
      where T : IDisposable {
    compositeDisposable.Add(disposable);
    return disposable;
  }
}
