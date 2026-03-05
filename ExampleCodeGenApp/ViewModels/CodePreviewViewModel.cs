namespace ExampleCodeGenApp.ViewModels;

using System.Linq;
using System.Reactive.Linq;
using ExampleCodeGenApp.Model.Compiler;
using ExampleCodeGenApp.Model.Compiler.Error;
using ReactiveUI;

public class CodePreviewViewModel : ReactiveObject {
  #region Code
  public IStatement Code {
    get => _code;
    set => this.RaiseAndSetIfChanged(ref _code, value);
  }
  private IStatement _code;
  #endregion

  #region CompilerError
  public string CompilerError {
    get => _compilerError;
    set => this.RaiseAndSetIfChanged(ref _compilerError, value);
  }
  private string _compilerError;
  #endregion

  #region CompiledCode
  private readonly ObservableAsPropertyHelper<string> _compiledCode;
  public string CompiledCode => _compiledCode.Value;
  #endregion

  public CodePreviewViewModel() {
    this.WhenAnyValue(vm => vm.Code).Where(c => c != null)
        .Select(c => {
          CompilerError = "";
          var ctx = new CompilerContext();

          try {
            return c.Compile(ctx);
          } catch (CompilerException e) {
            var trace = string.Join("\n", ctx.VariablesScopesStack.Select(s => s.Identifier));
            CompilerError = e.Message + "\nProblem is near:\n" + trace;
            return "";
          }
        })
        .ToProperty(this, vm => vm.CompiledCode, out _compiledCode);
  }
}
