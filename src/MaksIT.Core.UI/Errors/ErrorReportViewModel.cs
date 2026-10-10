using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;


namespace MaksIT.Core.UI.Errors;

/// <summary>Copyable crash report shown by <see cref="ErrorWindow"/>.</summary>
public sealed partial class ErrorReportViewModel : ObservableObject {
  public ErrorReportViewModel(string report, DesktopErrorText text) {
    Report = report ?? "";
    Text = text;
  }

  public string Report { get; }

  public DesktopErrorText Text { get; }

  public ICommand? CopyCommand { get; set; }

  public ICommand? CloseCommand { get; set; }

  [ObservableProperty]
  private string copyStatus = "";

  public void MarkCopied() =>
    CopyStatus = Text.CopiedLabel;
}
