namespace ExampleCodeGenApp.ViewModels;

using System;
using System.Linq;
using RxVoid = ReactiveUI.Primitives.RxVoid;
using System.Reactive.Linq;
using ExampleCodeGenApp.Model.Compiler;
using MoonSharp.Interpreter;
using ReactiveUI;

public class CodeSimViewModel : ReactiveObject {
  #region Code
  public IStatement Code {
    get => _code;
    set => this.RaiseAndSetIfChanged(ref _code, value);
  }
  private IStatement _code;
  #endregion

  #region Output
  public string Output {
    get => _output;
    set => this.RaiseAndSetIfChanged(ref _output, value);
  }
  private string _output;
  #endregion

  public ReactiveCommand<RxVoid, RxVoid> RunScript { get; }
  public ReactiveCommand<RxVoid, RxVoid> ClearOutput { get; }

  public CodeSimViewModel() {
    RunScript = ReactiveCommand.Create(() => {
      var script = new Script();
      script.Globals["print"] = (Action<string>)Print;
      var source = Code.Compile(new CompilerContext());
      script.DoString(source);
    },
        this.WhenAnyValue(vm => vm.Code).Select(code => code != null));

    ClearOutput = ReactiveCommand.Create(() => { Output = ""; });
  }

  public void Print(string msg) => Output += msg + "\n";
}
