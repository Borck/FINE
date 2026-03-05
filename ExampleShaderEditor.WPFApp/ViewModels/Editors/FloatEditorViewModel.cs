namespace ExampleShaderEditorApp.ViewModels.Editors;

using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using ExampleShaderEditorApp.Model;
using ExampleShaderEditorApp.Views;
using FINE.Toolkit.ValueNode;
using ReactiveUI;

public class FloatEditorViewModel : ValueEditorViewModel<ShaderFunc> {
  static FloatEditorViewModel() {
    Splat.Locator.CurrentMutable.Register(() => new FloatEditorView(), typeof(IViewFor<FloatEditorViewModel>));
  }

  #region FloatValue
  private float _floatValue;
  public float FloatValue {
    get => _floatValue;
    set => this.RaiseAndSetIfChanged(ref _floatValue, value);
  }
  #endregion

  public FloatEditorViewModel() {
    this.WhenAnyValue(vm => vm.FloatValue)
        .Select(v => new ShaderFunc(() => v.ToString(CultureInfo.InvariantCulture)))
        .BindTo(this, vm => vm.Value);
  }
}
