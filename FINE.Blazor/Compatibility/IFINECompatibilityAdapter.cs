namespace FINE.Blazor.Compatibility;

using FINE.Blazor.Models;

public interface IFINECompatibilityAdapter {
  FINEDocument Export(NetworkModel network);

  NetworkModel Import(FINEDocument document);
}
