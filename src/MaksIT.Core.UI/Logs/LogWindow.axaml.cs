using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;


namespace MaksIT.Core.UI.Logs;

public partial class LogWindow : Window {
  public LogWindow() {
    InitializeComponent();
  }

  public static Task ShowAsync(Window owner, DesktopLogOptions options) {
    ArgumentNullException.ThrowIfNull(owner);
    ArgumentNullException.ThrowIfNull(options);
    var text = options.ResolveText();
    var window = new LogWindow {
      Title = text.Title,
      Icon = owner.Icon,
      DataContext = new LogViewModel(options.Directory, text),
      WindowStartupLocation = WindowStartupLocation.CenterOwner
    };

    return window.ShowDialog(owner);
  }

  private async void OnCopyClick(object? sender, RoutedEventArgs e) {
    if (DataContext is not LogViewModel logs)
      return;

    var clipboard = Clipboard;

    if (clipboard is null)
      return;

    try {
      await clipboard.SetTextAsync(logs.Report);
      logs.MarkCopied();
    }
    catch {
    }
  }

  private void OnCloseClick(object? sender, RoutedEventArgs e) =>
    Close();
}
