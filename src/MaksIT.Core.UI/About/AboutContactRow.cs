using System.Windows.Input;


namespace MaksIT.Core.UI.About;

/// <summary>One contact row in the About window.</summary>
public sealed class AboutContactRow {
  public AboutContactRow(AppContact contact, ICommand open) {
    Role = contact.Role;
    Address = contact.Address;
    Uri = contact.Uri;
    Open = open;
  }

  public string Role { get; }

  public string Address { get; }

  public string Uri { get; }

  public ICommand Open { get; }
}
