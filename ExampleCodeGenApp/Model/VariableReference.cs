namespace ExampleCodeGenApp.Model;

using ExampleCodeGenApp.Model.Compiler;
using ExampleCodeGenApp.Model.Compiler.Error;

public class VariableReference<T> : ITypedExpression<T> {
  public ITypedVariableDefinition<T> LocalVariable { get; set; }

  public string Compile(CompilerContext context) {
    if (!context.IsInScope(LocalVariable)) {
      throw new VariableOutOfScopeException(LocalVariable.VariableName);
    }
    return LocalVariable.VariableName;
  }
}
