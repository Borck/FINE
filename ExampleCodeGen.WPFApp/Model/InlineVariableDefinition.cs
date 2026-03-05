namespace ExampleCodeGenApp.Model;

using ExampleCodeGenApp.Model.Compiler;

public class InlineVariableDefinition<T> : ITypedVariableDefinition<T> {
  public string VariableName { get; private set; }
  public ITypedExpression<T> Value { get; set; }

  public string Compile(CompilerContext context) {
    VariableName = context.FindFreeVariableName();
    context.AddVariableToCurrentScope(this);
    return $"{VariableName} = {Value.Compile(context)}";
  }
}
