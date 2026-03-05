namespace ExampleCodeGenApp.Model.Compiler.Error;

using System;

public class CompilerException : Exception {
  public CompilerException(string msg) : base(msg) { }

  public CompilerException(string msg, Exception inner) : base(msg, inner) { }
}
