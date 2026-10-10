using System.Runtime.InteropServices;
using System.Text;


namespace MaksIT.Core.Desktop;

/// <summary>Formats unhandled exceptions for a copyable dialog and a crash log file.</summary>
public static class CrashReport {
  public static string Capture(
    Exception exception,
    string productLine,
    string brand,
    Func<string> logDirectory,
    Action<string>? alsoWrite = null) {
    ArgumentNullException.ThrowIfNull(exception);
    ArgumentNullException.ThrowIfNull(logDirectory);
    var body = Format(exception, productLine, brand);
    alsoWrite?.Invoke(body);
    var path = TryWrite(body, logDirectory);

    if (string.IsNullOrWhiteSpace(path))
      return body;

    return body + Environment.NewLine + Environment.NewLine + "Log: " + path;
  }

  public static string Format(Exception exception, string productLine, string brand) {
    ArgumentNullException.ThrowIfNull(exception);
    var text = new StringBuilder();
    text.AppendLine(productLine);
    text.AppendLine(brand);
    text.AppendLine(DateTimeOffset.UtcNow.ToString("u"));
    text.AppendLine(RuntimeInformation.OSDescription);
    text.AppendLine(RuntimeInformation.FrameworkDescription);
    text.AppendLine(Environment.OSVersion.VersionString);
    text.AppendLine((Environment.Is64BitProcess ? "64-bit" : "32-bit") + " process");
    text.AppendLine();
    AppendException(text, exception);

    return text.ToString().TrimEnd();
  }

  private static string? TryWrite(string report, Func<string> logDirectory) {
    try {
      var directory = logDirectory();
      Directory.CreateDirectory(directory);
      var name = "crash-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff") + "-" + Environment.ProcessId + ".txt";
      var path = Path.Combine(directory, name);
      File.WriteAllText(path, report ?? "");

      return path;
    }
    catch {
      return null;
    }
  }

  private static void AppendException(StringBuilder text, Exception exception) {
    var seen = new HashSet<Exception>();
    var current = exception;
    var depth = 0;

    while (current is not null && seen.Add(current)) {
      if (depth > 0)
        text.AppendLine().AppendLine("--- inner ---");

      text.AppendLine(current.GetType().FullName);
      text.AppendLine(current.Message);

      if (current.HResult != 0)
        text.AppendLine("HResult: 0x" + current.HResult.ToString("X8"));

      if (!string.IsNullOrWhiteSpace(current.Source))
        text.AppendLine("Source: " + current.Source);

      if (!string.IsNullOrWhiteSpace(current.TargetSite?.ToString()))
        text.AppendLine("Target: " + current.TargetSite);

      if (!string.IsNullOrWhiteSpace(current.StackTrace))
        text.AppendLine(current.StackTrace);

      if (current is AggregateException aggregate) {
        foreach (var inner in aggregate.InnerExceptions) {
          if (inner is null || ReferenceEquals(inner, current.InnerException) || !seen.Add(inner))
            continue;

          text.AppendLine().AppendLine("--- aggregate ---");
          text.AppendLine(inner.ToString());
        }
      }

      current = current.InnerException;
      depth++;
    }
  }
}
