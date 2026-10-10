namespace MaksIT.Core.Desktop;

public sealed record ReleaseNote(string Version, IReadOnlyList<string> Added);

public static class ReleaseNotes {
  public static IReadOnlyList<ReleaseNote> AddedSince(string markdown, string? seenVersion, string currentVersion) {
    if (!TryParse(currentVersion, out var current))
      return [];

    var seenKnown = TryParse(seenVersion, out var seen);

    if (seenKnown && seen.CompareTo(current) >= 0)
      return [];

    var matches = new List<ReleaseNote>();

    foreach (var section in Parse(markdown)) {
      if (section.Number.CompareTo(current) > 0 || section.Added.Count == 0)
        continue;

      if (!seenKnown) {
        if (section.Number.CompareTo(current) != 0)
          continue;
      }
      else if (section.Number.CompareTo(seen) <= 0)
        continue;

      matches.Add(new ReleaseNote(section.Text, section.Added));
    }

    matches.Sort((left, right) => {
      TryParse(right.Version, out var newer);
      TryParse(left.Version, out var older);

      return newer.CompareTo(older);
    });

    return matches;
  }

  private static List<ParsedSection> Parse(string markdown) {
    var sections = new List<ParsedSection>();
    ParsedSection? current = null;
    var inAdded = false;

    foreach (var raw in markdown.Split('\n')) {
      var line = raw.TrimEnd('\r').Trim();

      if (line.StartsWith("## ", StringComparison.Ordinal)) {
        if (current is not null)
          sections.Add(current);

        current = null;
        inAdded = false;

        if (!TryHeading(line, out var text, out var number))
          continue;

        current = new ParsedSection(text, number);
        inAdded = true;

        continue;
      }

      if (current is null)
        continue;

      if (line.StartsWith("### ", StringComparison.Ordinal)) {
        inAdded = line.Equals("### Added", StringComparison.OrdinalIgnoreCase);

        continue;
      }

      if (inAdded && line.StartsWith("- ", StringComparison.Ordinal))
        current.Added.Add(line[2..].Trim());
    }

    if (current is not null)
      sections.Add(current);

    return sections;
  }

  private static bool TryHeading(string line, out string text, out NoteVersion number) {
    text = "";
    number = new NoteVersion(new Version(), null);

    if (!line.StartsWith("## [", StringComparison.Ordinal))
      return false;

    var end = line.IndexOf(']', 4);

    if (end < 0)
      return false;

    text = line[4..end];

    if (text.Equals("Unreleased", StringComparison.OrdinalIgnoreCase))
      return false;

    return TryParse(text, out number);
  }

  private static bool TryParse(string? version, out NoteVersion number) {
    number = new NoteVersion(new Version(), null);

    if (string.IsNullOrWhiteSpace(version))
      return false;

    var text = version.Trim();
    var plus = text.IndexOf('+', StringComparison.Ordinal);

    if (plus >= 0)
      text = text[..plus];

    string? pre = null;
    var dash = text.IndexOf('-', StringComparison.Ordinal);

    if (dash >= 0) {
      pre = text[(dash + 1)..];
      text = text[..dash];

      if (string.IsNullOrWhiteSpace(pre))
        pre = null;
    }

    if (!Version.TryParse(text, out var parsed))
      return false;

    number = new NoteVersion(parsed, pre);

    return true;
  }

  private readonly record struct NoteVersion(Version Number, string? Pre) : IComparable<NoteVersion> {
    public int CompareTo(NoteVersion other) {
      var compared = Number.CompareTo(other.Number);

      if (compared != 0)
        return compared;

      if (Pre is null && other.Pre is null)
        return 0;

      if (Pre is null)
        return 1;

      if (other.Pre is null)
        return -1;

      return string.Compare(Pre, other.Pre, StringComparison.Ordinal);
    }
  }

  private sealed class ParsedSection(string text, NoteVersion number) {
    public string Text { get; } = text;

    public NoteVersion Number { get; } = number;

    public List<string> Added { get; } = [];
  }
}
