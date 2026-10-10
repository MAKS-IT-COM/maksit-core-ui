namespace MaksIT.Core.Desktop.Snapshots;

/// <summary>
/// App-view snapshot run requested with <c>--screenshots</c>.
/// Null when that flag is absent. Other arguments are left for the host.
/// </summary>
public sealed class AppViewSnapshotOptions {
  public required string Directory { get; init; }

  /// <summary>View ids from <c>--views</c>. Empty when the host should choose its own default set.</summary>
  public IReadOnlyList<string> Views { get; init; } = [];

  public int SettleMilliseconds { get; init; } = 600;

  /// <summary>
  /// Reads <c>--screenshots</c>, <c>--views</c>, and <c>--settle-ms</c>.
  /// Returns false when one of those flags is present but incomplete.
  /// </summary>
  public static bool TryParse(IReadOnlyList<string>? args, out AppViewSnapshotOptions? options, out string? error) {
    options = null;
    error = null;

    if (args is null || args.Count == 0)
      return true;

    string? directory = null;
    IReadOnlyList<string>? views = null;
    var settle = 600;
    var requested = false;

    for (var i = 0; i < args.Count; i++) {
      switch (args[i]) {
        case "--screenshots":
          requested = true;

          if (!TryTake(args, ref i, "--screenshots", out directory, out error))
            return false;

          break;
        case "--views":
          if (!TryTake(args, ref i, "--views", out var viewList, out error))
            return false;

          views = viewList!
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

          if (views.Count == 0) {
            error = "Pass at least one view id after --views.";

            return false;
          }

          break;
        case "--settle-ms":
          if (!TryTake(args, ref i, "--settle-ms", out var settleText, out error))
            return false;

          if (!int.TryParse(settleText, out settle) || settle < 0) {
            error = "Pass a non-negative number of milliseconds after --settle-ms.";

            return false;
          }

          break;
      }
    }

    if (!requested)
      return true;

    if (string.IsNullOrWhiteSpace(directory)) {
      error = "Pass a directory after --screenshots.";

      return false;
    }

    options = new AppViewSnapshotOptions {
      Directory = directory,
      Views = views ?? [],
      SettleMilliseconds = settle
    };

    return true;
  }

  private static bool TryTake(
    IReadOnlyList<string> args,
    ref int index,
    string flag,
    out string? value,
    out string? error) {
    value = null;
    error = null;
    var next = index + 1;

    if (next >= args.Count || args[next].StartsWith("--", StringComparison.Ordinal)) {
      error = $"Pass a value after {flag}.";

      return false;
    }

    index = next;
    value = args[next];

    return true;
  }
}
