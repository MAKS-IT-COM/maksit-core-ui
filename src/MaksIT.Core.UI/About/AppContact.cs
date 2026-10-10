namespace MaksIT.Core.UI.About;

/// <summary>A labeled address shown in the About window. <see cref="Uri"/> opens the mail client.</summary>
public readonly record struct AppContact(string Role, string Address) {
  public string Uri =>
    "mailto:" + Address;
}
