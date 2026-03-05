namespace ExampleShaderEditorApp.ViewModels.Nodes;

using DynamicData;
using ExampleShaderEditorApp.Model;
using ExampleShaderEditorApp.ViewModels.Editors;
using NodeNetwork.Views;
using ReactiveUI;
using Splat;

public class ColorNodeViewModel : ShaderNodeViewModel {
  static ColorNodeViewModel() {
    Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<ColorNodeViewModel>));
  }

  public ShaderNodeOutputViewModel ColorOutput { get; } = new ShaderNodeOutputViewModel();


  private async void LoadIcon() =>
    // This reloads the icon for each instance of the viewmodel
    // A more efficient implementation would load this once into a static field, then reuse it in each vm instance.
    HeaderIcon = await BitmapLoader.Current.LoadFromResource(
        "pack://application:,,,/Resources/Icons/colorwheel.png", 20, 20);

  public ColorNodeViewModel() {
    Name = "Color";
    Category = NodeCategory.Misc;
    LoadIcon();

    var editor = new ColorEditorViewModel();
    ColorOutput.Editor = editor;
    ColorOutput.ReturnType = typeof(Vec3);
    ColorOutput.Value = editor.ValueChanged;
    Outputs.Add(ColorOutput);
  }
}
