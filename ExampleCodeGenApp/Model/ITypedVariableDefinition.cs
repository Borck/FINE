namespace ExampleCodeGenApp.Model;

using ExampleCodeGenApp.Model.Compiler;

public interface IVariableDefinition : IStatement {
  string VariableName { get; }
}

public interface ITypedVariableDefinition<T> : IVariableDefinition {
}
