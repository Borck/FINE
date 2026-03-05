namespace ExampleCodeGenApp.Model;

using ExampleCodeGenApp.Model.Compiler;

public class StringLiteral : ITypedExpression<string> {
  public string Value { get; set; }

  public string Compile(CompilerContext ctx) => $"\"{Value}\"";
}
