using System.ComponentModel;
using MaksIT.Core.Desktop;

namespace MaksIT.Core.Desktop.Tests;

public class CrashReportTests {
  [Fact]
  public void Format_IncludesProductBrandAndExceptionChain() {
    var inner = new ArgumentException("missing name");
    inner.Source = "Host";
    var error = new InvalidOperationException("failed", inner);

    var report = CrashReport.Format(error, "MaksIT App", "MAKS-IT");
    var lines = report.Split(Environment.NewLine);

    Assert.Equal("MaksIT App", lines[0]);
    Assert.Equal("MAKS-IT", lines[1]);
    Assert.Contains("process", lines[6]);
    Assert.Contains(typeof(InvalidOperationException).FullName!, report);
    Assert.Contains("failed", report);
    Assert.Contains("--- inner ---", report);
    Assert.Contains(typeof(ArgumentException).FullName!, report);
    Assert.Contains("missing name", report);
    Assert.Contains("Source: Host", report);
    Assert.DoesNotContain("Log:", report);
  }

  [Fact]
  public void Format_IncludesHResultSourceAndAggregateInners() {
    var aggregate = new AggregateException(
      "many",
      new InvalidOperationException("one"),
      new IOException("two"));
    aggregate.Source = "Worker";

    var report = CrashReport.Format(aggregate, "Product", "Brand");

    Assert.Contains("HResult: 0x", report);
    Assert.Contains("Source: Worker", report);
    Assert.Contains("--- aggregate ---", report);
    Assert.Contains("one", report);
    Assert.Contains("two", report);
  }

  [Fact]
  public void Format_IncludesWin32HResult() {
    var error = new Win32Exception(5, "access denied");

    var report = CrashReport.Format(error, "Product", "Brand");

    Assert.Contains("access denied", report);
    Assert.Contains("HResult: 0x", report);
  }

  [Fact]
  public void Format_RejectsNullException() {
    Assert.Throws<ArgumentNullException>(() => CrashReport.Format(null!, "Product", "Brand"));
  }

  [Fact]
  public void Capture_WritesLogAndNotifiesBeforeThePathIsAppended() {
    var directory = NewDirectory();
    string? written = null;

    try {
      var report = CrashReport.Capture(
        new InvalidOperationException("boom"),
        "Product",
        "Brand",
        () => directory,
        body => written = body);

      Assert.NotNull(written);
      Assert.DoesNotContain("Log:", written);
      Assert.Contains("boom", written);
      Assert.Contains("Log: ", report);
      var path = report.Split("Log: ", StringSplitOptions.None)[1].Trim();
      Assert.StartsWith(directory, path);
      Assert.Contains(Environment.ProcessId.ToString(), Path.GetFileName(path));
      Assert.Equal(written, File.ReadAllText(path));
    }
    finally {
      Directory.Delete(directory, recursive: true);
    }
  }

  [Fact]
  public void Capture_ReturnsTheReportWhenTheLogCannotBeWritten() {
    var report = CrashReport.Capture(
      new InvalidOperationException("boom"),
      "Product",
      "Brand",
      () => throw new IOException("disk"));

    Assert.Contains("boom", report);
    Assert.DoesNotContain("Log:", report);
  }

  [Fact]
  public void Capture_RejectsNullLogDirectory() {
    Assert.Throws<ArgumentNullException>(() =>
      CrashReport.Capture(new InvalidOperationException("boom"), "Product", "Brand", null!));
  }

  private static string NewDirectory() =>
    Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "maksit-core-ui-tests", Guid.NewGuid().ToString("n"))).FullName;
}
