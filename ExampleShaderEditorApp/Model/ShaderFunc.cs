namespace ExampleShaderEditorApp.Model;

using System;

public class ShaderFunc {
  public Func<string> CompilationFunc { get; set; }

  public ShaderFunc(Func<string> compilationFunc) {
    CompilationFunc = compilationFunc;
  }

  public string Compile() => CompilationFunc();
}
