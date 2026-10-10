using Avalonia.Controls;
using Avalonia.Interactivity;
using MaksIT.Core.Desktop;


namespace MaksIT.Core.UI.WhatsNew;

public partial class WhatsNewWindow : Window {
  public WhatsNewWindow() {
    InitializeComponent();
  }

  public static async Task ShowIfNeededAsync(
    Window owner,
    string markdown,
    string currentVersion,
    string? seenVersion,
    bool settingsExisted,
    Action<string> remember) {
    ArgumentNullException.ThrowIfNull(owner);
    ArgumentNullException.ThrowIfNull(remember);

    if (string.IsNullOrWhiteSpace(currentVersion))
      return;

    if (!settingsExisted) {
      remember(currentVersion);

      return;
    }

    var notes = ReleaseNotes.AddedSince(markdown, seenVersion, currentVersion);

    if (notes.Count == 0)
      return;

    if (await ShowAsync(owner, notes))
      remember(currentVersion);
  }

  public static async Task<bool> ShowAsync(Window owner, IReadOnlyList<ReleaseNote> notes) {
    ArgumentNullException.ThrowIfNull(owner);
    var model = new WhatsNewViewModel(notes);
    var window = new WhatsNewWindow {
      DataContext = model,
      Icon = owner.Icon,
      WindowStartupLocation = WindowStartupLocation.CenterOwner
    };
    await window.ShowDialog(owner);

    return model.DoNotShowAgain;
  }

  private void OnCloseClick(object? sender, RoutedEventArgs e) =>
    Close();
}
