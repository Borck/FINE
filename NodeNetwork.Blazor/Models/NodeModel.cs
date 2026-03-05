namespace NodeNetwork.Blazor.Models;

public sealed class NodeModel {
  public Guid Id { get; init; } = Guid.NewGuid();

  public string Name { get; set; } = "Untitled";

  public CanvasPoint Position { get; set; }

  public CanvasSize Size { get; set; } = new(280, 180);

  public bool IsCollapsed { get; set; }

  public bool IsSelected { get; set; }

  public bool CanBeRemovedByUser { get; set; } = true;

  public List<NodePortModel> Inputs { get; } = [];

  public List<NodePortModel> Outputs { get; } = [];

  public IEnumerable<NodePortModel> GetPorts() => Inputs.Concat(Outputs);
}
