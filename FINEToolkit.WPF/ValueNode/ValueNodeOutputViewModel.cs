namespace FINE.Toolkit.ValueNode;

using System;
using System.Reactive.Linq;
using ReactiveUI.Primitives.Concurrency;
using FINE.ViewModels;
using FINE.Views;
using ReactiveUI;

/// <summary>
/// Non-generic surface of <see cref="ValueNodeOutputViewModel{T}"/>, used by
/// <see cref="ValueNodeInputViewModel{T}"/> to accept a connection from an output whose value type is merely
/// *assignable* to the input's own type (e.g. a <c>double</c> output feeding an <c>object</c> input), rather
/// than requiring the two closed generic types to match exactly. Every value crosses this boundary boxed,
/// which costs nothing extra for reference types and is unavoidable for the value-type case this interface
/// exists to support in the first place.
/// </summary>
public interface IValueNodeOutput {
  /// <summary>The CLR type this output's <see cref="ValueNodeOutputViewModel{T}"/> was closed over.</summary>
  Type ValueType { get; }

  /// <summary>Same sequence as <see cref="ValueNodeOutputViewModel{T}.Value"/>, boxed.</summary>
  IObservable<object> ValueBoxed { get; }

  /// <summary>Same value as <see cref="ValueNodeOutputViewModel{T}.CurrentValue"/>, boxed.</summary>
  object CurrentValueBoxed { get; }
}

/// <summary>
/// A viewmodel for a node output that produces a value based on the inputs.
/// </summary>
/// <typeparam name="T">The type of object produced by this output.</typeparam>
public class ValueNodeOutputViewModel<T> : NodeOutputViewModel, IValueNodeOutput {
  static ValueNodeOutputViewModel() {
    NNViewRegistrar.AddRegistration(() => new NodeOutputView(), typeof(IViewFor<ValueNodeOutputViewModel<T>>));
  }

  #region Value
  /// <summary>
  /// Observable that produces the value every time it changes.
  /// </summary>
  public IObservable<T> Value {
    get => _value;
    set => this.RaiseAndSetIfChanged(ref _value, value);
  }
  private IObservable<T> _value;
  #endregion

  #region CurrentValue
  /// <summary>
  /// The latest value produced by this output.
  /// </summary>
  public T CurrentValue => _currentValue.Value;
  private readonly ObservableAsPropertyHelper<T> _currentValue;
  #endregion

  #region IValueNodeOutput
  Type IValueNodeOutput.ValueType => typeof(T);
  IObservable<object> IValueNodeOutput.ValueBoxed =>
      (Value ?? Observable.Empty<T>()).Select(v => (object)v);
  object IValueNodeOutput.CurrentValueBoxed => CurrentValue;
  #endregion

  public ValueNodeOutputViewModel() {
    this.WhenAnyObservable(vm => vm.Value).ToProperty(this, vm => vm.CurrentValue, out _currentValue, false, Sequencer.Immediate);
  }
}
