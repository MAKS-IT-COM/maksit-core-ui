namespace MaksIT.Core.UI.Errors;

/// <summary>Labels for the shared error window. Hosts can supply localized text.</summary>
public sealed record DesktopErrorText {
  public string Title { get; init; } = "Error";

  public string Hint { get; init; } = "Something went wrong. The details below are also saved to a crash log.";

  public string CopyLabel { get; init; } = "Copy";

  public string CloseLabel { get; init; } = "Close";

  public string CopiedLabel { get; init; } = "Copied";
}
