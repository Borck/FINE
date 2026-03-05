namespace ExampleShaderEditorApp.ViewModels.Nodes;

using System.Reactive.Linq;
using DynamicData;
using ExampleShaderEditorApp.Model;
using NodeNetwork.Views;
using ReactiveUI;

public class TimeNodeViewModel : ShaderNodeViewModel {
  static TimeNodeViewModel() {
    Splat.Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<TimeNodeViewModel>));
  }

  public ShaderNodeOutputViewModel Result { get; } = new ShaderNodeOutputViewModel();

  public TimeNodeViewModel() {
    Name = "Time";
    Category = NodeCategory.Misc;

    Result.Name = "Seconds";
    Result.ReturnType = typeof(float);
    Result.Value = Observable.Return(new ShaderFunc(() => "seconds"));
    Outputs.Add(Result);
  }
}
