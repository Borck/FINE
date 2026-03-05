namespace ExampleShaderEditorApp.ViewModels.Nodes;

using System.Reactive.Linq;
using DynamicData;
using ExampleShaderEditorApp.Model;
using NodeNetwork.Views;
using ReactiveUI;
using Splat;

public class GeometryNodeViewModel : ShaderNodeViewModel {
  static GeometryNodeViewModel() {
    Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<GeometryNodeViewModel>));
  }

  public ShaderNodeOutputViewModel VertexPositionOutput { get; } = new ShaderNodeOutputViewModel();
  public ShaderNodeOutputViewModel NormalOutput { get; } = new ShaderNodeOutputViewModel();
  public ShaderNodeOutputViewModel CameraOutput { get; } = new ShaderNodeOutputViewModel();

  private async void LoadIcons() {
    // This reloads the icons for each instance of the viewmodel
    // A more efficient implementation would load these once into a static field, then reuse it in each vm instance.
    VertexPositionOutput.Icon = await BitmapLoader.Current.LoadFromResource(
        "pack://application:,,,/Resources/Icons/pos.png", 20, 20);
    NormalOutput.Icon = await BitmapLoader.Current.LoadFromResource(
        "pack://application:,,,/Resources/Icons/norm.png", 20, 20);
    CameraOutput.Icon = await BitmapLoader.Current.LoadFromResource(
        "pack://application:,,,/Resources/Icons/eye.png", 20, 20);
  }

  public GeometryNodeViewModel() {
    Name = "Geometry";
    Category = NodeCategory.Misc;
    LoadIcons();

    VertexPositionOutput.Name = "Position";
    VertexPositionOutput.ReturnType = typeof(Vec3);
    VertexPositionOutput.Value = Observable.Return(new ShaderFunc(() => "pos"));
    VertexPositionOutput.Editor = null;
    Outputs.Add(VertexPositionOutput);

    NormalOutput.Name = "Normal";
    NormalOutput.ReturnType = typeof(Vec3);
    NormalOutput.Value = Observable.Return(new ShaderFunc(() => "norm"));
    NormalOutput.Editor = null;
    Outputs.Add(NormalOutput);

    CameraOutput.Name = "Camera";
    CameraOutput.ReturnType = typeof(Vec3);
    CameraOutput.Value = Observable.Return(new ShaderFunc(() => "cam"));
    CameraOutput.Editor = null;
    Outputs.Add(CameraOutput);
  }
}
