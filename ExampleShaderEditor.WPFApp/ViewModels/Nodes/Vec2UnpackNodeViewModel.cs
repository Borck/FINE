namespace ExampleShaderEditorApp.ViewModels.Nodes;

using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using ExampleShaderEditorApp.Model;
using FINE.Views;
using ReactiveUI;

public class Vec2UnpackNodeViewModel : ShaderNodeViewModel {
  static Vec2UnpackNodeViewModel() {
    Splat.Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<Vec2UnpackNodeViewModel>));
  }

  public ShaderNodeInputViewModel VectorInput { get; } = new ShaderNodeInputViewModel(typeof(Vec2));

  public ShaderNodeOutputViewModel X { get; } = new ShaderNodeOutputViewModel();
  public ShaderNodeOutputViewModel Y { get; } = new ShaderNodeOutputViewModel();

  public Vec2UnpackNodeViewModel() {
    Name = "Unpack Vec2";
    Category = NodeCategory.Vector;

    VectorInput.Name = "Vec2";
    VectorInput.Editor = null;
    Inputs.Add(VectorInput);

    X.Name = "X";
    X.ReturnType = typeof(float);
    X.Value = this.WhenAnyValue(vm => vm.VectorInput.Value).Select(v => v == null ? null : new ShaderFunc(() => $"({v.Compile()}).x"));
    Outputs.Add(X);

    Y.Name = "Y";
    Y.ReturnType = typeof(float);
    Y.Value = this.WhenAnyValue(vm => vm.VectorInput.Value).Select(v => v == null ? null : new ShaderFunc(() => $"({v.Compile()}).y"));
    Outputs.Add(Y);
  }
}
