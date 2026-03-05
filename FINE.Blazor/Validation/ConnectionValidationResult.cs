namespace FINE.Blazor.Validation;

public sealed record ConnectionValidationResult(bool IsAllowed, string? Error) {
  public static ConnectionValidationResult Allowed() => new(true, null);

  public static ConnectionValidationResult Rejected(string error) => new(false, error);
}
