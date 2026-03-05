namespace FINETests;

using System.Collections.Generic;
using System.Linq;

using DynamicData;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FINE.ViewModels;

[TestClass]
public class EndpointGroupingTests {
  [TestMethod]
  public void TestNewNodeHasNoGroups() {
    var node = new NodeViewModel();

    Assert.IsFalse(node.VisibleEndpointGroups.Any());
  }

  [TestMethod]
  public void TestAddingUngroupedEndpoints() {
    var node = new NodeViewModel();

    node.Inputs.Add(new NodeInputViewModel());
    node.Outputs.Add(new NodeOutputViewModel());

    Assert.IsFalse(node.VisibleEndpointGroups.Any());
  }

  [TestMethod]
  public void TestGroupedEndpoints() {
    var node = new NodeViewModel();

    var groupA = new EndpointGroup();
    var groupB = new EndpointGroup();

    var inputA1 = new NodeInputViewModel { Group = groupA };
    var inputA2 = new NodeInputViewModel { Group = groupA };
    var outputA1 = new NodeOutputViewModel { Group = groupA };
    var outputA2 = new NodeOutputViewModel { Group = groupA };

    var inputB1 = new NodeInputViewModel { Group = groupB };
    var inputB2 = new NodeInputViewModel { Group = groupB };
    var outputB1 = new NodeOutputViewModel { Group = groupB };
    var outputB2 = new NodeOutputViewModel { Group = groupB };

    node.Inputs.Add(inputB1);

    Assert.IsTrue(node.VisibleInputs.Count == 0);
    Assert.IsTrue(node.VisibleEndpointGroups.Count == 1);
    var groupBViewModel = node.VisibleEndpointGroups[0];
    Assert.IsTrue(groupBViewModel.Group == groupB);
    Assert.IsTrue(groupBViewModel.VisibleInputs.Count == 1);
    Assert.AreEqual(inputB1, groupBViewModel.VisibleInputs.Items[0]);

    node.Outputs.Add(outputB2);

    Assert.IsTrue(node.VisibleInputs.Count == 0);
    Assert.IsTrue(node.VisibleEndpointGroups.Count == 1);
    groupBViewModel = node.VisibleEndpointGroups[0];
    Assert.IsTrue(groupBViewModel.Group == groupB);
    Assert.IsTrue(groupBViewModel.VisibleInputs.Count == 1);
    Assert.AreEqual(inputB1, groupBViewModel.VisibleInputs.Items[0]);
    Assert.IsTrue(groupBViewModel.VisibleOutputs.Count == 1);
    Assert.AreEqual(outputB2, groupBViewModel.VisibleOutputs.Items[0]);

    node.Inputs.AddRange(new[] { inputA1, inputB2, inputA2 });
    node.Outputs.AddRange(new[] { outputB1, outputA1, outputA2 });

    Assert.IsTrue(node.VisibleInputs.Count == 0);
    Assert.IsTrue(node.VisibleEndpointGroups.Count == 2);
    groupBViewModel = node.VisibleEndpointGroups[0];
    Assert.IsTrue(groupBViewModel.Group == groupB);
    Assert.IsTrue(groupBViewModel.VisibleInputs.Count == 2);
    Assert.AreEqual(inputB1, groupBViewModel.VisibleInputs.Items[0]);
    Assert.AreEqual(inputB2, groupBViewModel.VisibleInputs.Items.ElementAt(1));
    Assert.IsTrue(groupBViewModel.VisibleOutputs.Count == 2);
    Assert.AreEqual(outputB2, groupBViewModel.VisibleOutputs.Items[0]);
    Assert.AreEqual(outputB1, groupBViewModel.VisibleOutputs.Items.ElementAt(1));

    var groupAViewModel = node.VisibleEndpointGroups[1];
    Assert.IsTrue(groupAViewModel.Group == groupA);
    Assert.IsTrue(groupAViewModel.VisibleInputs.Count == 2);
    Assert.AreEqual(inputA1, groupAViewModel.VisibleInputs.Items[0]);
    Assert.AreEqual(inputA2, groupAViewModel.VisibleInputs.Items.ElementAt(1));
    Assert.IsTrue(groupAViewModel.VisibleOutputs.Count == 2);
    Assert.AreEqual(outputA1, groupAViewModel.VisibleOutputs.Items[0]);
    Assert.AreEqual(outputA2, groupAViewModel.VisibleOutputs.Items.ElementAt(1));

    node.Inputs.Remove(inputB1);

    Assert.IsTrue(node.VisibleInputs.Count == 0);
    Assert.IsTrue(node.VisibleEndpointGroups.Count == 2);
    groupBViewModel = node.VisibleEndpointGroups[0];
    Assert.IsTrue(groupBViewModel.Group == groupB);
    Assert.IsTrue(groupBViewModel.VisibleInputs.Count == 1);
    Assert.AreEqual(inputB2, groupBViewModel.VisibleInputs.Items[0]);
    Assert.IsTrue(groupBViewModel.VisibleOutputs.Count == 2);
    Assert.AreEqual(outputB2, groupBViewModel.VisibleOutputs.Items[0]);
    Assert.AreEqual(outputB1, groupBViewModel.VisibleOutputs.Items.ElementAt(1));

    groupAViewModel = node.VisibleEndpointGroups[1];
    Assert.IsTrue(groupAViewModel.Group == groupA);
    Assert.IsTrue(groupAViewModel.VisibleInputs.Count == 2);
    Assert.AreEqual(inputA1, groupAViewModel.VisibleInputs.Items[0]);
    Assert.AreEqual(inputA2, groupAViewModel.VisibleInputs.Items.ElementAt(1));
    Assert.IsTrue(groupAViewModel.VisibleOutputs.Count == 2);
    Assert.AreEqual(outputA1, groupAViewModel.VisibleOutputs.Items[0]);
    Assert.AreEqual(outputA2, groupAViewModel.VisibleOutputs.Items.ElementAt(1));

    node.Inputs.Remove(inputB2);
    node.Outputs.RemoveMany(new[] { outputB1, outputB2 });

    Assert.IsTrue(node.VisibleInputs.Count == 0);
    Assert.IsTrue(node.VisibleEndpointGroups.Count == 1);

    groupAViewModel = node.VisibleEndpointGroups[0];
    Assert.IsTrue(groupAViewModel.Group == groupA);
    Assert.IsTrue(groupAViewModel.VisibleInputs.Count == 2);
    Assert.AreEqual(inputA1, groupAViewModel.VisibleInputs.Items[0]);
    Assert.AreEqual(inputA2, groupAViewModel.VisibleInputs.Items.ElementAt(1));
    Assert.IsTrue(groupAViewModel.VisibleOutputs.Count == 2);
    Assert.AreEqual(outputA1, groupAViewModel.VisibleOutputs.Items[0]);
    Assert.AreEqual(outputA2, groupAViewModel.VisibleOutputs.Items.ElementAt(1));
  }

  [TestMethod]
  public void TestNestedGroups() {
    var node = new NodeViewModel();

    var groupA = new EndpointGroup { Name = "Group A" };
    var groupB = new EndpointGroup { Name = "Group B" };
    var groupC = new EndpointGroup(groupA) { Name = "Group C" };
    var groupD = new EndpointGroup(groupB) { Name = "Group D" };

    var inputC = new NodeInputViewModel { Group = groupC, Name = "Input C" };
    var outputC = new NodeOutputViewModel { Group = groupC, Name = "Output C" };

    var inputD = new NodeInputViewModel { Group = groupD, Name = "Input D" };
    var outputD = new NodeOutputViewModel { Group = groupD, Name = "Output D" };

    node.Inputs.Add(inputC);
    node.Inputs.Add(inputD);
    node.Outputs.Add(outputC);
    node.Outputs.Add(outputD);

    Assert.IsTrue(node.VisibleInputs.Count == 0);
    Assert.IsTrue(node.VisibleOutputs.Count == 0);
    Assert.IsTrue(node.VisibleEndpointGroups.Count == 2);

    var groupAViewModel = node.VisibleEndpointGroups[0];
    Assert.AreEqual(groupA, groupAViewModel.Group);
    Assert.IsTrue(groupAViewModel.VisibleInputs.Count == 0);
    Assert.IsTrue(groupAViewModel.VisibleOutputs.Count == 0);
    Assert.IsTrue(groupAViewModel.Children.Count == 1);
    var groupCViewModel = groupAViewModel.Children[0];
    Assert.AreEqual(groupC, groupCViewModel.Group);
    Assert.IsTrue(groupCViewModel.VisibleInputs.Count == 1);
    Assert.AreEqual(inputC, groupCViewModel.VisibleInputs.Items[0]);
    Assert.IsTrue(groupCViewModel.VisibleOutputs.Count == 1);
    Assert.AreEqual(outputC, groupCViewModel.VisibleOutputs.Items[0]);

    var groupBViewModel = node.VisibleEndpointGroups[1];
    Assert.AreEqual(groupB, groupBViewModel.Group);
    Assert.IsTrue(groupBViewModel.VisibleInputs.Count == 0);
    Assert.IsTrue(groupBViewModel.VisibleOutputs.Count == 0);
    Assert.IsTrue(groupBViewModel.Children.Count == 1);
    var groupDViewModel = groupBViewModel.Children[0];
    Assert.AreEqual(groupD, groupDViewModel.Group);
    Assert.IsTrue(groupDViewModel.VisibleInputs.Count == 1);
    Assert.AreEqual(inputD, groupDViewModel.VisibleInputs.Items[0]);
    Assert.IsTrue(groupDViewModel.VisibleOutputs.Count == 1);
    Assert.AreEqual(outputD, groupDViewModel.VisibleOutputs.Items[0]);

    node.Inputs.Remove(inputC);
    node.Outputs.Remove(outputC);

    Assert.IsTrue(node.VisibleInputs.Count == 0);
    Assert.IsTrue(node.VisibleOutputs.Count == 0);
    Assert.IsTrue(node.VisibleEndpointGroups.Count == 1);

    groupBViewModel = node.VisibleEndpointGroups[0];
    Assert.AreEqual(groupB, groupBViewModel.Group);
    Assert.IsTrue(groupBViewModel.VisibleInputs.Count == 0);
    Assert.IsTrue(groupBViewModel.VisibleOutputs.Count == 0);
    Assert.IsTrue(groupBViewModel.Children.Count == 1);
    groupDViewModel = groupBViewModel.Children[0];
    Assert.AreEqual(groupD, groupDViewModel.Group);
    Assert.IsTrue(groupDViewModel.VisibleInputs.Count == 1);
    Assert.AreEqual(inputD, groupDViewModel.VisibleInputs.Items[0]);
    Assert.IsTrue(groupDViewModel.VisibleOutputs.Count == 1);
    Assert.AreEqual(outputD, groupDViewModel.VisibleOutputs.Items[0]);
  }

  [TestMethod]
  public void TestCollapseWithGroups() {
    var node = new NodeViewModel();

    var groupA = new EndpointGroup { Name = "Group A" };
    var groupB = new EndpointGroup { Name = "Group B" };
    var groupC = new EndpointGroup(groupA) { Name = "Group C" };
    var groupD = new EndpointGroup(groupB) { Name = "Group D" };

    var inputC = new NodeInputViewModel { Group = groupC, Name = "Input C" };
    var outputC = new NodeOutputViewModel { Group = groupC, Name = "Output C" };

    var inputD = new NodeInputViewModel { Group = groupD, Name = "Input D" };
    var outputD = new NodeOutputViewModel { Group = groupD, Name = "Output D" };

    node.Inputs.Add(inputC);
    node.Inputs.Add(inputD);
    node.Outputs.Add(outputC);
    node.Outputs.Add(outputD);

    var network = new NetworkViewModel();
    network.Nodes.Add(node);
    network.Connections.Add(network.ConnectionFactory(inputC, new NodeOutputViewModel()));

    node.IsCollapsed = true;

    Assert.IsTrue(node.VisibleInputs.Count == 0);
    Assert.IsTrue(node.VisibleOutputs.Count == 0);
    Assert.IsTrue(node.VisibleEndpointGroups.Count == 1);

    var groupAViewModel = node.VisibleEndpointGroups[0];
    Assert.AreEqual(groupA, groupAViewModel.Group);
    Assert.IsTrue(groupAViewModel.VisibleInputs.Count == 0);
    Assert.IsTrue(groupAViewModel.VisibleOutputs.Count == 0);
    Assert.IsTrue(groupAViewModel.Children.Count == 1);
    var groupCViewModel = groupAViewModel.Children[0];
    Assert.AreEqual(groupC, groupCViewModel.Group);
    Assert.IsTrue(groupCViewModel.VisibleInputs.Count == 1);
    Assert.AreEqual(inputC, groupCViewModel.VisibleInputs.Items[0]);
    Assert.IsTrue(groupCViewModel.VisibleOutputs.Count == 0);
  }
}
