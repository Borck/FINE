namespace ExampleCodeGenApp.Model;

using ExampleCodeGenApp.Model.Compiler;

public class IntLiteral : ITypedExpression<int> {
  public int Value { get; set; }

  public string Compile(CompilerContext context) => Value.ToString();
}
