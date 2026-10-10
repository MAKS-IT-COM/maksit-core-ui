using MaksIT.Core.Desktop;

namespace MaksIT.Core.Desktop.Tests;

public class ReleaseNotesTests {
  private const string Markdown = """
    ## [Unreleased]

    ### Added

    - ignore unreleased

    ## [2.0.0]

    ### Added

    - too new

    ## [1.2.0] - 2026-10-10

    ### Added

    - newer feature

    ### Fixed

    - not an added line

    ## [1.1.0]

    ### Added

    - middle feature

    ## [1.0.0]

    ### Added

    - old feature

    ## [1.0.0-beta]

    ### Added

    - beta feature

    ## [1.0.0-alpha]

    ### Added

    - alpha feature

    ## [not-a-version]

    ### Added

    - skip heading

    ## [0.9.0]

    ### Changed

    - no added section
    """;

  [Fact]
  public void AddedSince_ReturnsNotesBetweenSeenAndCurrentNewestFirst() {
    var notes = ReleaseNotes.AddedSince(Markdown, "1.0.0", "1.2.0");

    Assert.Equal(["1.2.0", "1.1.0"], notes.Select(note => note.Version).ToArray());
    Assert.Equal(["newer feature"], notes[0].Added);
    Assert.Equal(["middle feature"], notes[1].Added);
  }

  [Fact]
  public void AddedSince_WhenNothingWasSeen_ReturnsOnlyTheCurrentVersion() {
    var notes = ReleaseNotes.AddedSince(Markdown, null, "1.1.0");

    Assert.Single(notes);
    Assert.Equal("1.1.0", notes[0].Version);
    Assert.Equal(["middle feature"], notes[0].Added);
  }

  [Fact]
  public void AddedSince_IncludesPrereleasesUntilTheRelease() {
    var notes = ReleaseNotes.AddedSince(Markdown, "1.0.0-alpha", "1.0.0");

    Assert.Equal(["1.0.0", "1.0.0-beta"], notes.Select(note => note.Version).ToArray());
  }

  [Fact]
  public void AddedSince_WhenSeenIsBlank_ReturnsOnlyTheCurrentVersion() {
    var notes = ReleaseNotes.AddedSince(Markdown, "  ", "1.0.0");

    Assert.Single(notes);
    Assert.Equal("1.0.0", notes[0].Version);
  }

  [Fact]
  public void AddedSince_StripsBuildMetadataBeforeComparing() {
    const string markdown = """
      ## [1.0.0+build.9]

      ### Added

      - build metadata
      """;

    var notes = ReleaseNotes.AddedSince(markdown, null, "1.0.0+build.9");

    Assert.Single(notes);
    Assert.Equal("1.0.0+build.9", notes[0].Version);
    Assert.Equal(["build metadata"], notes[0].Added);
  }

  [Theory]
  [InlineData("1.2.0", "1.2.0")]
  [InlineData("1.3.0", "1.2.0")]
  [InlineData(null, "not-a-version")]
  public void AddedSince_ReturnsEmptyWhenCurrentIsUnknownOrAlreadySeen(string? seen, string current) {
    Assert.Empty(ReleaseNotes.AddedSince(Markdown, seen, current));
  }

  [Fact]
  public void AddedSince_AcceptsTheRepositoryChangelog() {
    var directory = new DirectoryInfo(AppContext.BaseDirectory);

    while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "CHANGELOG.md")))
      directory = directory.Parent;

    Assert.NotNull(directory);
    var markdown = File.ReadAllText(Path.Combine(directory.FullName, "CHANGELOG.md"));
    var notes = ReleaseNotes.AddedSince(markdown, null, "0.1.0");

    Assert.Single(notes);
    Assert.Equal("0.1.0", notes[0].Version);
    Assert.Contains(notes[0].Added, line => line.Contains("MaksIT.Core.Desktop", StringComparison.Ordinal));
  }
}
