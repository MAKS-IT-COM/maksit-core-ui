using Avalonia.Controls;
using Avalonia.Interactivity;


namespace MaksIT.Core.UI.About;

/// <summary>Shared About window. The host passes product facts and its own window icon.</summary>
public partial class AboutWindow : Window {
  public AboutWindow() {
    InitializeComponent();
  }

  public static Task ShowAsync(Window owner, DesktopAppInfo info) {
    ArgumentNullException.ThrowIfNull(owner);
    ArgumentNullException.ThrowIfNull(info);
    var window = new AboutWindow {
      Title = info.WindowTitle,
      DataContext = new AboutViewModel(info),
      WindowStartupLocation = WindowStartupLocation.CenterOwner,
      Icon = owner.Icon
    };

    return window.ShowDialog(owner);
  }

  private void OnCloseClick(object? sender, RoutedEventArgs e) =>
    Close();
}
