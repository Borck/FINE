namespace ExampleCodeGenApp.Model;

using ExampleCodeGenApp.Model.Compiler;

public class LocalVariableDefinition<T> : ITypedVariableDefinition<T> {
  public string VariableName { get; private set; }
  public string Value { get; set; }

  public string Compile(CompilerContext context) {
    VariableName = context.FindFreeVariableName();
    context.AddVariableToCurrentScope(this);
    return $"local {VariableName} = {Value}\n";
  }
}
