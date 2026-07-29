namespace ExampleShaderEditorApp.ViewModels.Nodes;

using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using ExampleShaderEditorApp.Model;
using FINE.Views;
using ReactiveUI;
using Splat;

public class GeometryNodeViewModel : ShaderNodeViewModel {
  static GeometryNodeViewModel() {
    Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<GeometryNodeViewModel>));
  }

  private static readonly Task<IBitmap> VertexPositionIconTask = BitmapLoader.Current.LoadFromResource(
      "pack://application:,,,/Resources/Icons/pos.png", 20, 20);
  private static readonly Task<IBitmap> NormalIconTask = BitmapLoader.Current.LoadFromResource(
      "pack://application:,,,/Resources/Icons/norm.png", 20, 20);
  private static readonly Task<IBitmap> CameraIconTask = BitmapLoader.Current.LoadFromResource(
      "pack://application:,,,/Resources/Icons/eye.png", 20, 20);

  public ShaderNodeOutputViewModel VertexPositionOutput { get; } = new ShaderNodeOutputViewModel();
  public ShaderNodeOutputViewModel NormalOutput { get; } = new ShaderNodeOutputViewModel();
  public ShaderNodeOutputViewModel CameraOutput { get; } = new ShaderNodeOutputViewModel();

  private async void LoadIcons() {
    VertexPositionOutput.Icon = await VertexPositionIconTask;
    NormalOutput.Icon = await NormalIconTask;
    CameraOutput.Icon = await CameraIconTask;
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
