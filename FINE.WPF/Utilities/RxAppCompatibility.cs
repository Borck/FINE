namespace ReactiveUI;

#if NET8_0_OR_GREATER
using System.Reactive.Concurrency;

public static class RxApp {
  public static IScheduler MainThreadScheduler => CurrentThreadScheduler.Instance;
}
#endif
