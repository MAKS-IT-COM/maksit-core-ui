using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;


namespace MaksIT.Core.UI.About;

/// <summary>About window content for one <see cref="DesktopAppInfo"/>.</summary>
public sealed partial class AboutViewModel {
  public AboutViewModel(DesktopAppInfo info) {
    ArgumentNullException.ThrowIfNull(info);
    Info = info;
    OpenContactCommand = new RelayCommand<string>(OpenContact);
    Contacts = info.Contacts
      .Select(contact => new AboutContactRow(contact, OpenContactCommand))
      .ToArray();
  }

  public DesktopAppInfo Info { get; }

  public IReadOnlyList<AboutContactRow> Contacts { get; }

  public ICommand OpenContactCommand { get; }

  public bool HasVersion => !string.IsNullOrWhiteSpace(Info.Version);

  public string VersionText =>
    string.IsNullOrWhiteSpace(Info.VersionLabel)
      ? Info.Version
      : $"{Info.VersionLabel} {Info.Version}";

  public bool HasCopyright => !string.IsNullOrWhiteSpace(Info.Copyright);

  public bool HasLicense => !string.IsNullOrWhiteSpace(Info.License);

  public bool HasLegal => HasCopyright || HasLicense;

  public bool HasCredits => !string.IsNullOrWhiteSpace(Info.Credits);

  public bool HasContributors => Info.Contributors.Count > 0;

  public bool HasPeople => HasCredits || HasContributors;

  public bool HasContacts => Contacts.Count > 0;

  public bool HasSite =>
    !string.IsNullOrWhiteSpace(Info.Site) && !string.IsNullOrWhiteSpace(Info.SiteUri);

  public bool HasLinks => HasContacts || HasSite;

  [RelayCommand]
  private void OpenSite() =>
    OpenUrl(Info.SiteUri);

  private void OpenContact(string? uri) {
    if (string.IsNullOrWhiteSpace(uri))
      return;

    OpenUrl(uri);
  }

  private static void OpenUrl(string url) {
    try {
      Process.Start(new ProcessStartInfo {
        FileName = url,
        UseShellExecute = true
      });
    }
    catch {
    }
  }
}
