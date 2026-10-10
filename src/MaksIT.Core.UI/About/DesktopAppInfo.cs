namespace MaksIT.Core.UI.About;

/// <summary>Product facts the shared About window displays. Each desktop app fills this in.</summary>
public sealed record DesktopAppInfo {
  public required string Brand { get; init; }

  public required string ProductName { get; init; }

  public required string Summary { get; init; }

  public string Version { get; init; } = "";

  /// <summary>Primary author. Shown under <see cref="CreditsLabel"/>.</summary>
  public string Credits { get; init; } = "";

  /// <summary>
  /// People besides the author. The section is hidden when this is empty,
  /// which is the usual case when one person maintains the app.
  /// </summary>
  public IReadOnlyList<string> Contributors { get; init; } = [];

  public string License { get; init; } = "";

  public string Copyright { get; init; } = "";

  public string Site { get; init; } = "maks-it.com";

  public string SiteUri { get; init; } = "https://maks-it.com";

  public string Title { get; init; } = "";

  public string VersionLabel { get; init; } = "Version";

  public string CreditsLabel { get; init; } = "Credits";

  public string ContributorsLabel { get; init; } = "Contributors";

  public string LicenseLabel { get; init; } = "License";

  public string CopyrightLabel { get; init; } = "Copyright";

  public string CloseLabel { get; init; } = "Close";

  public IReadOnlyList<AppContact> Contacts { get; init; } = [];

  public string WindowTitle =>
    string.IsNullOrWhiteSpace(Title) ? "About " + ProductName : Title;
}
