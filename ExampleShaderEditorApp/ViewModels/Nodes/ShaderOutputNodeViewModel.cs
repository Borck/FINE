namespace ExampleShaderEditorApp.ViewModels.Nodes;

using DynamicData;
using ExampleShaderEditorApp.Model;
using NodeNetwork.Views;
using ReactiveUI;

public class ShaderOutputNodeViewModel : ShaderNodeViewModel {
  static ShaderOutputNodeViewModel() {
    Splat.Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<ShaderOutputNodeViewModel>));
  }

  public ShaderNodeInputViewModel ColorInput { get; } = new ShaderNodeInputViewModel(typeof(Vec3));

  public ShaderOutputNodeViewModel() {
    Name = "Shader Output";
    Category = NodeCategory.Misc;
    CanBeRemovedByUser = false;

    ColorInput.Name = "Color";
    Inputs.Add(ColorInput);
  }
}
