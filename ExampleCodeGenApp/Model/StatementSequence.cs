namespace ExampleCodeGenApp.Model;

using System.Collections.Generic;
using ExampleCodeGenApp.Model.Compiler;

public class StatementSequence : IStatement {
  public List<IStatement> Statements { get; } = new List<IStatement>();

  public StatementSequence() { }

  public StatementSequence(IEnumerable<IStatement> statements) {
    Statements.AddRange(statements);
  }

  public string Compile(CompilerContext context) {
    var result = "";
    foreach (var statement in Statements) {
      result += statement.Compile(context);
      result += "\n";
    }
    return result;
  }
}
