namespace FINE;

using System;
using System.Collections.Generic;
using Splat;

/// <summary>
/// A locator is used to find the correct view corresponding to a viewmodel.
/// In ReactiveUI, usually Splat is used, but others exist. This class acts as an intermediate registrar.
/// It gathers registrations and registers them to the preferred locator.
/// </summary>
public sealed class NNViewRegistrar {
  private static readonly List<Tuple<Func<object>, Type>> _pendingRegistrations = [];
  private static Action<Func<object>, Type> _registerAction;

  public static void AddRegistration(Func<object> factory, Type serviceType) {
    if (factory == null) {
      throw new ArgumentNullException(nameof(factory));
    } else {
      ArgumentNullException.ThrowIfNull(serviceType);
    }

    if (_registerAction == null) {
      _pendingRegistrations.Add(Tuple.Create(factory, serviceType));
    } else {
      _registerAction(factory, serviceType);
    }
  }

  public static void RegisterToLocator(Action<Func<object>, Type> newRegisterAction) {
    if (newRegisterAction == null) {
      throw new ArgumentNullException(nameof(newRegisterAction));
    } else if (_registerAction != null) {
      throw new InvalidOperationException("A locator has already been set");
    }

    _registerAction = newRegisterAction;
    foreach (var t in _pendingRegistrations) {
      _registerAction(t.Item1, t.Item2);
    }
    _pendingRegistrations.Clear();
  }

  /// <summary>
  /// Register all FINE.WPF view/viewmodel pairs to Locator.CurrentMutable.
  /// </summary>
  public static void RegisterSplat() =>
    RegisterToLocator((f, t) => Locator.CurrentMutable.Register(f, t));
}
