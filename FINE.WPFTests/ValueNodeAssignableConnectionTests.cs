namespace FINETests;

using System;
using System.Reactive.Subjects;
using DynamicData;
using FINE.Toolkit.ValueNode;
using FINE.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Pins the fix for a closed-generic connection check that rejected a perfectly assignable connection -
/// e.g. a <c>double</c> output into an <c>object</c> input - because <c>ValueNodeOutputViewModel&lt;double&gt;</c>
/// and <c>ValueNodeOutputViewModel&lt;object&gt;</c> are unrelated closed generic types even though double
/// boxes to object. See ValueNodeInputViewModel's ConnectionValidator and IValueNodeOutput.
/// </summary>
[TestClass]
public class ValueNodeAssignableConnectionTests {
  [TestMethod]
  public void ConnectionValidator_AcceptsAnAssignableButNotIdenticalOutputType() {
    var nodeA = new NodeViewModel();
    var outputA = new ValueNodeOutputViewModel<double> { Value = new Subject<double>() };
    nodeA.Outputs.Add(outputA);

    var nodeB = new NodeViewModel();
    var inputB = new ValueNodeInputViewModel<object>();
    nodeB.Inputs.Add(inputB);

    var network = new NetworkViewModel();
    network.Nodes.AddRange([nodeA, nodeB]);

    var pending = new PendingConnectionViewModel(network) { Output = outputA };
    var result = inputB.ConnectionValidator(pending);

    Assert.IsTrue(result.IsValid);
  }

  [TestMethod]
  public void ConnectionValidator_RejectsATrulyIncompatibleOutputType() {
    var nodeA = new NodeViewModel();
    var outputA = new ValueNodeOutputViewModel<string> { Value = new Subject<string>() };
    nodeA.Outputs.Add(outputA);

    var nodeB = new NodeViewModel();
    var inputB = new ValueNodeInputViewModel<int>();
    nodeB.Inputs.Add(inputB);

    var network = new NetworkViewModel();
    network.Nodes.AddRange([nodeA, nodeB]);

    var pending = new PendingConnectionViewModel(network) { Output = outputA };
    var result = inputB.ConnectionValidator(pending);

    Assert.IsFalse(result.IsValid);
  }

  [TestMethod]
  public void Value_PropagatesBoxedValuesFromAnAssignableButNotIdenticalOutputType() {
    var nodeA = new NodeViewModel();
    var source = new Subject<double>();
    var outputA = new ValueNodeOutputViewModel<double> { Value = source };
    nodeA.Outputs.Add(outputA);

    var nodeB = new NodeViewModel();
    var inputB = new ValueNodeInputViewModel<object>();
    nodeB.Inputs.Add(inputB);

    var network = new NetworkViewModel();
    network.Nodes.AddRange([nodeA, nodeB]);
    network.Connections.Add(network.ConnectionFactory(inputB, outputA));

    source.OnNext(42.5);

    Assert.AreEqual(42.5, inputB.Value);
  }

  [TestMethod]
  public void Value_StillUsesTheZeroBoxingFastPath_WhenTypesMatchExactly() {
    var nodeA = new NodeViewModel();
    var source = new Subject<int>();
    var outputA = new ValueNodeOutputViewModel<int> { Value = source };
    nodeA.Outputs.Add(outputA);

    var nodeB = new NodeViewModel();
    var inputB = new ValueNodeInputViewModel<int>();
    nodeB.Inputs.Add(inputB);

    var network = new NetworkViewModel();
    network.Nodes.AddRange([nodeA, nodeB]);
    network.Connections.Add(network.ConnectionFactory(inputB, outputA));

    source.OnNext(7);

    Assert.AreEqual(7, inputB.Value);
  }
}
