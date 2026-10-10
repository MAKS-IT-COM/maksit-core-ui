using MaksIT.Core.Desktop.Snapshots;

namespace MaksIT.Core.Desktop.Tests.Snapshots;

public class AppViewSnapshotOptionsTests {
  [Fact]
  public void TryParse_WithoutArguments_LeavesTheHostAlone() {
    Assert.True(AppViewSnapshotOptions.TryParse(null, out var missing, out var missingError));
    Assert.Null(missing);
    Assert.Null(missingError);
    Assert.True(AppViewSnapshotOptions.TryParse([], out var empty, out var emptyError));
    Assert.Null(empty);
    Assert.Null(emptyError);
  }

  [Fact]
  public void TryParse_IgnoresUnrelatedArguments() {
    Assert.True(AppViewSnapshotOptions.TryParse(["--other", "value"], out var options, out var error));
    Assert.Null(options);
    Assert.Null(error);
  }

  [Fact]
  public void TryParse_ReadsDirectoryViewsAndSettleTime() {
    var parsed = AppViewSnapshotOptions.TryParse(
      ["--settle-ms", "0", "--views", "about, logs", "--screenshots", "shots"],
      out var options,
      out var error);

    Assert.True(parsed);
    Assert.Null(error);
    Assert.NotNull(options);
    Assert.Equal("shots", options.Directory);
    Assert.Equal(["about", "logs"], options.Views);
    Assert.Equal(0, options.SettleMilliseconds);
  }

  [Fact]
  public void TryParse_UsesDefaultViewsAndSettleTime() {
    Assert.True(AppViewSnapshotOptions.TryParse(["--screenshots", "shots"], out var options, out var error));
    Assert.Null(error);
    Assert.NotNull(options);
    Assert.Empty(options.Views);
    Assert.Equal(600, options.SettleMilliseconds);
  }

  [Theory]
  [InlineData("--screenshots")]
  [InlineData("--screenshots", "--views")]
  public void TryParse_RequiresAValueAfterScreenshots(params string[] args) {
    Assert.False(AppViewSnapshotOptions.TryParse(args, out var options, out var error));
    Assert.Null(options);
    Assert.Equal("Pass a value after --screenshots.", error);
  }

  [Fact]
  public void TryParse_RejectsABlankDirectory() {
    Assert.False(AppViewSnapshotOptions.TryParse(["--screenshots", "  "], out var options, out var error));
    Assert.Null(options);
    Assert.Equal("Pass a directory after --screenshots.", error);
  }

  [Fact]
  public void TryParse_RequiresAtLeastOneView() {
    Assert.False(AppViewSnapshotOptions.TryParse(["--views", " , "], out var options, out var error));
    Assert.Null(options);
    Assert.Equal("Pass at least one view id after --views.", error);
  }

  [Theory]
  [InlineData("-1", "Pass a non-negative number of milliseconds after --settle-ms.")]
  [InlineData("soon", "Pass a non-negative number of milliseconds after --settle-ms.")]
  [InlineData("--screenshots", "Pass a value after --settle-ms.")]
  public void TryParse_RejectsABadSettleTime(string value, string expected) {
    Assert.False(AppViewSnapshotOptions.TryParse(["--settle-ms", value], out var options, out var error));
    Assert.Null(options);
    Assert.Equal(expected, error);
  }
}
