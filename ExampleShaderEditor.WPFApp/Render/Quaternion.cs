namespace ExampleShaderEditorApp.Render;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

public class Quaternion {
  public static Quaternion Identity => new Quaternion();

  public UnitVector3D RotationAxis { get; set; } = UnitVector3D.Create(0, 0, 1);
  public Angle Angle { get; set; } = Angle.FromRadians(0);

  public Matrix<double> GetMatrix() => Matrix3D.RotationAroundArbitraryVector(RotationAxis, Angle);

  public Quaternion Invert() => new Quaternion {
    Angle = -Angle,
    RotationAxis = RotationAxis
  };
}
