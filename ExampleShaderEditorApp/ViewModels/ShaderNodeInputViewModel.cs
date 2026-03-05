namespace ExampleShaderEditorApp.ViewModels;

using System;
using System.Linq;
using ExampleShaderEditorApp.Model;
using NodeNetwork;
using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using ReactiveUI;

public class ShaderNodeInputViewModel : ValueNodeInputViewModel<ShaderFunc> {
  static ShaderNodeInputViewModel() {
    Splat.Locator.CurrentMutable.Register(() => new NodeInputView(), typeof(IViewFor<ShaderNodeInputViewModel>));
  }

  public ShaderNodeInputViewModel(params Type[] acceptedTypes) {
    Editor = null;
    ConnectionValidator = con => {
      var type = ((ShaderNodeOutputViewModel)con.Output).ReturnType;
      var isValidType = acceptedTypes.Contains(type);
      return new ConnectionValidationResult(isValidType,
          isValidType ? null : new ErrorMessageViewModel($"Incorrect type, got {type.Name} but need one of {string.Join(", ", acceptedTypes.Select(t => t.Name))}"));
    };
  }
}
