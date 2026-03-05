namespace NodeNetwork.Blazor.Models;

/// <summary>
/// A point in the network canvas coordinate system.
/// </summary>
public readonly record struct CanvasPoint(double X, double Y) {
  public static CanvasPoint operator +(CanvasPoint left, CanvasPoint right) => new(left.X + right.X, left.Y + right.Y);

  public static CanvasPoint operator -(CanvasPoint left, CanvasPoint right) => new(left.X - right.X, left.Y - right.Y);
}

/// <summary>
/// A size in the network canvas coordinate system.
/// </summary>
public readonly record struct CanvasSize(double Width, double Height);
