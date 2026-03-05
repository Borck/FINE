namespace ExampleCodeGenApp.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using ExampleCodeGenApp.Model.Compiler;

public class FunctionCall : IStatement {
  public string FunctionName { get; set; }
  public List<IExpression> Parameters { get; } = new List<IExpression>();

  public string Compile(CompilerContext context) => $"{FunctionName}({String.Join(", ", Parameters.Select(p => p.Compile(context)))})\n";
}
