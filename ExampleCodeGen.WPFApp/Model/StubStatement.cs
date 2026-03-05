namespace ExampleCodeGenApp.Model;

using ExampleCodeGenApp.Model.Compiler;

public class StubStatement : IStatement {
  public string Compile(CompilerContext context) => "";
}
