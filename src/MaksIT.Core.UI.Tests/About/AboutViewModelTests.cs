using MaksIT.Core.UI.About;

namespace MaksIT.Core.UI.Tests.About;

public class AboutViewModelTests {
  [Fact]
  public void Info_HidesEmptySectionsAndBuildsTheWindowTitle() {
    var info = Sample();
    var model = new AboutViewModel(info);

    Assert.Equal("About Sample", info.WindowTitle);
    Assert.False(model.HasVersion);
    Assert.Equal("Version ", model.VersionText);
    Assert.False(model.HasCopyright);
    Assert.False(model.HasLicense);
    Assert.False(model.HasLegal);
    Assert.False(model.HasCredits);
    Assert.False(model.HasContributors);
    Assert.False(model.HasPeople);
    Assert.False(model.HasContacts);
    Assert.True(model.HasSite);
    Assert.True(model.HasLinks);
    Assert.Empty(model.Contacts);
  }

  [Fact]
  public void Info_ShowsFilledSectionsAndContactRows() {
    var info = Sample() with {
      Title = "About this app",
      Version = "1.2.3",
      VersionLabel = "Build",
      Credits = "Maksym",
      Contributors = ["Ada"],
      License = "MIT",
      Copyright = "2026",
      Contacts = [new AppContact("Author", "a@example.com")]
    };
    var model = new AboutViewModel(info);

    Assert.Equal("About this app", info.WindowTitle);
    Assert.True(model.HasVersion);
    Assert.Equal("Build 1.2.3", model.VersionText);
    Assert.True(model.HasLegal);
    Assert.True(model.HasPeople);
    Assert.True(model.HasContacts);
    Assert.True(model.HasLinks);
    var row = Assert.Single(model.Contacts);
    Assert.Equal("Author", row.Role);
    Assert.Equal("a@example.com", row.Address);
    Assert.Equal("mailto:a@example.com", row.Uri);
    Assert.Same(model.OpenContactCommand, row.Open);
  }

  [Fact]
  public void VersionText_UsesTheVersionAloneWhenTheLabelIsBlank() {
    var model = new AboutViewModel(Sample() with { Version = "1.0", VersionLabel = " " });

    Assert.Equal("1.0", model.VersionText);
  }

  [Fact]
  public void OpenContactCommand_IgnoresABlankAddress() {
    var model = new AboutViewModel(Sample());

    model.OpenContactCommand.Execute(null);
    model.OpenContactCommand.Execute("  ");
  }

  [Fact]
  public void Constructor_RejectsNullInfo() {
    Assert.Throws<ArgumentNullException>(() => new AboutViewModel(null!));
  }

  private static DesktopAppInfo Sample() => new() {
    Brand = "MAKS-IT",
    ProductName = "Sample",
    Summary = "A sample app."
  };
}
