namespace ExampleCodeGenApp.Model.Compiler;

public interface IExpression {
  string Compile(CompilerContext context);
}

public interface ITypedExpression<T> : IExpression {
}
