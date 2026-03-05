namespace NodeNetwork.Blazor.Models;

public sealed class NetworkModel {
  public List<NodeModel> Nodes { get; } = [];

  public List<ConnectionModel> Connections { get; } = [];

  public double Zoom { get; set; } = 1.0;

  public CanvasPoint Pan { get; set; }
}
