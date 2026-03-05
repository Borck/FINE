namespace ExampleCodeGenApp.Model.Compiler.Error;

public class VariableOutOfScopeException : CompilerException {
  public string VariableName { get; }

  public VariableOutOfScopeException(string variableName)
      : base($"The variable '{variableName}' was referenced outside its scope.") {
    VariableName = variableName;
  }
}
