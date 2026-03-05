namespace ExampleShaderEditorApp.ViewModels.Nodes;

using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using ExampleShaderEditorApp.Model;
using NodeNetwork.Views;
using ReactiveUI;

public class Vec3UnpackNodeViewModel : ShaderNodeViewModel {
  static Vec3UnpackNodeViewModel() {
    Splat.Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<Vec3UnpackNodeViewModel>));
  }

  public ShaderNodeInputViewModel VectorInput { get; } = new ShaderNodeInputViewModel(typeof(Vec3));

  public ShaderNodeOutputViewModel X { get; } = new ShaderNodeOutputViewModel();
  public ShaderNodeOutputViewModel Y { get; } = new ShaderNodeOutputViewModel();
  public ShaderNodeOutputViewModel Z { get; } = new ShaderNodeOutputViewModel();

  public Vec3UnpackNodeViewModel() {
    Name = "Unpack Vec3";
    Category = NodeCategory.Vector;

    VectorInput.Name = "Vec3";
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

    Z.Name = "Z";
    Z.ReturnType = typeof(float);
    Z.Value = this.WhenAnyValue(vm => vm.VectorInput.Value).Select(v => v == null ? null : new ShaderFunc(() => $"({v.Compile()}).z"));
    Outputs.Add(Z);
  }
}
