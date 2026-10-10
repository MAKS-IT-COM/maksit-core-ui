namespace MaksIT.Core.UI.Logs;

public sealed class DesktopLogText {
  public string Title { get; init; } = "Logs";

  public string CopyLabel { get; init; } = "Copy";

  public string CloseLabel { get; init; } = "Close";

  public string CopiedLabel { get; init; } = "Copied";
}

/// <summary>
/// Where a desktop app keeps its log files, and the labels for the shared log window.
/// </summary>
public sealed class DesktopLogOptions {
  public required Func<string> Directory { get; init; }

  public Func<DesktopLogText>? Text { get; init; }

  public DesktopLogText ResolveText() =>
    Text?.Invoke() ?? new DesktopLogText();
}
