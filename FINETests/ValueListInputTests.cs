namespace FINETests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FINE.Toolkit.ValueNode;
using FINE.ViewModels;
using ReactiveUI;

[TestClass]
public class ValueListInputTests {


  [TestMethod]
  public void TestValuePropagation() {
    //Setup
    var scheduler = new TestScheduler();

    var nodeA = new NodeViewModel();
    var sourceA = new Subject<int>();
    var outputA = new ValueNodeOutputViewModel<int> {
      Value = sourceA
    };
    nodeA.Outputs.Add(outputA);

    var nodeB = new NodeViewModel();
    var inputB = new ValueListNodeInputViewModel<int>();
    nodeB.Inputs.Add(inputB);

    var network = new NetworkViewModel();
    network.Nodes.AddRange(new[] { nodeA, nodeB });

    network.Connections.Add(network.ConnectionFactory(inputB, outputA));

    //Define actions
    scheduler.Schedule(TimeSpan.FromTicks(10), () => sourceA.OnNext(1));
    scheduler.Schedule(TimeSpan.FromTicks(20), () => sourceA.OnNext(0));
    scheduler.Schedule(TimeSpan.FromTicks(30), () => sourceA.OnNext(1));
    scheduler.Schedule(TimeSpan.FromTicks(40), () => sourceA.OnNext(0));
    scheduler.Schedule(TimeSpan.FromTicks(50), () => sourceA.OnNext(2));
    var actual = scheduler.Start(() => inputB.Values.Connect().QueryWhenChanged(), created: 0, subscribed: 0, disposed: 100);

    //Assert
    Assert.AreEqual(actual.Messages.Count, 6);
    Assert.AreEqual(actual.Messages[0].Time, 1);
    Assert.IsTrue(actual.Messages[0].Value.Value.SequenceEqual([0]));
    Assert.AreEqual(actual.Messages[1].Time, 10);
    Assert.IsTrue(actual.Messages[1].Value.Value.SequenceEqual(new[] { 1 }));
    Assert.AreEqual(actual.Messages[2].Time, 20);
    Assert.IsTrue(actual.Messages[2].Value.Value.SequenceEqual([0]));
    Assert.AreEqual(actual.Messages[3].Time, 30);
    Assert.IsTrue(actual.Messages[3].Value.Value.SequenceEqual(new[] { 1 }));
    Assert.AreEqual(actual.Messages[4].Time, 40);
    Assert.IsTrue(actual.Messages[4].Value.Value.SequenceEqual(new[] { 0 }));
    Assert.AreEqual(actual.Messages[5].Time, 50);
    Assert.IsTrue(actual.Messages[5].Value.Value.SequenceEqual(new[] { 2 }));
  }

  public class Example : ReactiveObject {
    private int _value;
    public int Value {
      get => _value;
      set => this.RaiseAndSetIfChanged(ref _value, value);
    }
  }

  [TestMethod]
  public void TestDDAutorefresh() {
    var list = new SourceList<Example>();
    var valueList = list.Connect()
        .AutoRefresh(e => e.Value)
        .Transform(e => e.Value, true)
        .AsObservableList();

    var obj = new Example { Value = 0 };
    list.Add(obj);
    obj.Value = 1;
    Assert.AreEqual(valueList.Items[0], 1);
  }



  [TestMethod]
  public void TestOutputListPassthrough() {
    //Setup
    var scheduler = new TestScheduler();

    var valNode = new NodeViewModel();
    var valOutput1 = new ValueNodeOutputViewModel<int> { Value = Observable.Return(1) };
    valNode.Outputs.Add(valOutput1);
    var valOutput2 = new ValueNodeOutputViewModel<int> { Value = Observable.Return(2) };
    valNode.Outputs.Add(valOutput2);
    var valOutput3 = new ValueNodeOutputViewModel<int> { Value = Observable.Return(3) };
    valNode.Outputs.Add(valOutput3);
    //

    var nodeA = new NodeViewModel();
    var inputA = new ValueListNodeInputViewModel<int>();
    nodeA.Inputs.Add(inputA);
    var outputA = new ValueNodeOutputViewModel<IObservableList<int>> {
      Value = Observable.Return(inputA.Values)
    };
    nodeA.Outputs.Add(outputA);

    var nodeB = new NodeViewModel();
    var inputB = new ValueListNodeInputViewModel<int>();
    nodeB.Inputs.Add(inputB);

    var network = new NetworkViewModel();
    network.Nodes.AddRange(new[] { nodeA, nodeB });

    network.Connections.Add(network.ConnectionFactory(inputB, outputA));

    //
    network.Connections.Add(network.ConnectionFactory(inputA, valOutput1));
    Assert.IsTrue(inputA.Values.Items.SequenceEqual([1]));
    Assert.IsTrue(inputB.Values.Items.SequenceEqual(inputA.Values.Items));
    var con2 = network.ConnectionFactory(inputA, valOutput2);
    network.Connections.Add(con2);
    Assert.IsTrue(inputB.Values.Items.SequenceEqual(inputA.Values.Items));
    network.Connections.Add(network.ConnectionFactory(inputA, valOutput3));
    Assert.IsTrue(inputB.Values.Items.SequenceEqual(inputA.Values.Items));
    network.Connections.Remove(con2);
    Assert.IsTrue(inputB.Values.Items.SequenceEqual(inputA.Values.Items));
  }
}
