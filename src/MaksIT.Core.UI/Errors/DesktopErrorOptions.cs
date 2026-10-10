namespace MaksIT.Core.UI.Errors;

/// <summary>
/// How a desktop app turns an exception into the text shown in <see cref="ErrorWindow"/>.
/// <see cref="Capture"/> writes the crash log. <see cref="SuppressWindow"/> skips the dialog
/// after the log is written, for headless worker processes.
/// </summary>
public sealed class DesktopErrorOptions {
  public required Func<Exception, string> Capture { get; init; }

  public Func<DesktopErrorText>? Text { get; init; }

  public Func<bool>? SuppressWindow { get; init; }

  public DesktopErrorText ResolveText() =>
    Text?.Invoke() ?? new DesktopErrorText();
}
