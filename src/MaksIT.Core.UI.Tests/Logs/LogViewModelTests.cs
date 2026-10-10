using MaksIT.Core.UI.Logs;

namespace MaksIT.Core.UI.Tests.Logs;

public class LogViewModelTests {
  [Fact]
  public void Refresh_SelectsTheNewestFileAndLoadsIt() {
    var directory = NewDirectory();

    try {
      var older = Path.Combine(directory, "older.txt");
      var newer = Path.Combine(directory, "newer.txt");
      File.WriteAllText(older, "old body");
      File.WriteAllText(newer, "new body");
      File.SetLastWriteTimeUtc(older, DateTime.UtcNow.AddHours(-2));
      File.SetLastWriteTimeUtc(newer, DateTime.UtcNow.AddHours(-1));
      var model = new LogViewModel(() => directory, new DesktopLogText { CopyLabel = "Copy log", CopiedLabel = "Done" });

      Assert.Equal(["newer.txt", "older.txt"], model.Files);
      Assert.Equal("newer.txt", model.SelectedFile);
      Assert.Equal("new body", model.Report);
      Assert.Equal("Copy log", model.CopyLabel);

      model.SelectedFile = "older.txt";
      Assert.Equal("old body", model.Report);
      Assert.Equal("", model.CopyStatus);
      model.MarkCopied();
      Assert.Equal("Done", model.CopyStatus);

      model.Refresh();
      Assert.Equal("older.txt", model.SelectedFile);
      Assert.Equal("old body", model.Report);
    }
    finally {
      Directory.Delete(directory, recursive: true);
    }
  }

  [Fact]
  public void Refresh_ClearsTheReportWhenTheDirectoryCannotBeRead() {
    var model = new LogViewModel(() => throw new IOException("missing"), new DesktopLogText());

    Assert.Empty(model.Files);
    Assert.Equal("", model.SelectedFile);
    Assert.Equal("", model.Report);
  }

  [Fact]
  public void Refresh_ShowsTheReadErrorForALockedFile() {
    var directory = NewDirectory();
    var path = Path.Combine(directory, "locked.txt");
    File.WriteAllText(path, "secret");

    try {
      using var locked = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
      var model = new LogViewModel(() => directory, new DesktopLogText());

      Assert.Equal("locked.txt", model.SelectedFile);
      Assert.False(string.IsNullOrWhiteSpace(model.Report));
      Assert.DoesNotContain("secret", model.Report);
    }
    finally {
      Directory.Delete(directory, recursive: true);
    }
  }

  [Fact]
  public void ResolveText_UsesHostLabelsOrDefaults() {
    var custom = new DesktopLogOptions {
      Directory = () => "logs",
      Text = () => new DesktopLogText { Title = "Crash logs" }
    };
    var defaults = new DesktopLogOptions { Directory = () => "logs" };

    Assert.Equal("Crash logs", custom.ResolveText().Title);
    Assert.Equal("Logs", defaults.ResolveText().Title);
    Assert.Equal("Copy", defaults.ResolveText().CopyLabel);
  }

  private static string NewDirectory() =>
    Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "maksit-core-ui-tests", Guid.NewGuid().ToString("n"))).FullName;
}
