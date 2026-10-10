using Avalonia.Controls;
using Avalonia.Input.Platform;
using CommunityToolkit.Mvvm.Input;


namespace MaksIT.Core.UI.Errors;

/// <summary>Crash report window. Copy and Close are commands so the host theme styles the buttons.</summary>
public partial class ErrorWindow : Window {
  public ErrorWindow() {
    InitializeComponent();
  }

  public void Attach(ErrorReportViewModel report) {
    DataContext = report;
    report.CopyCommand = new AsyncRelayCommand(CopyAsync);
    report.CloseCommand = new RelayCommand(Close);
  }

  private async Task CopyAsync() {
    if (DataContext is not ErrorReportViewModel report)
      return;

    var clipboard = Clipboard;

    if (clipboard is null)
      return;

    try {
      await clipboard.SetTextAsync(report.Report);
      report.MarkCopied();
    }
    catch {
    }
  }
}
