using MaksIT.Core.UI.Errors;

namespace MaksIT.Core.UI.Tests.Errors;

public class ErrorReportTests {
  [Fact]
  public void ResolveText_UsesTheHostLabels() {
    var options = new DesktopErrorOptions {
      Capture = _ => "saved",
      Text = () => new DesktopErrorText { Title = "Crash", CopiedLabel = "Done" }
    };

    var text = options.ResolveText();

    Assert.Equal("Crash", text.Title);
    Assert.Equal("Done", text.CopiedLabel);
  }

  [Fact]
  public void ResolveText_UsesEnglishDefaults() {
    var options = new DesktopErrorOptions { Capture = static error => error.ToString() };

    Assert.Equal("Error", options.ResolveText().Title);
    Assert.Equal("Copy", options.ResolveText().CopyLabel);
  }

  [Fact]
  public void ViewModel_TreatsANullReportAsEmptyAndMarksItCopied() {
    var model = new ErrorReportViewModel(null!, new DesktopErrorText { CopiedLabel = "Done" });

    Assert.Equal("", model.Report);
    Assert.Equal("", model.CopyStatus);
    model.MarkCopied();
    Assert.Equal("Done", model.CopyStatus);
  }

  [Fact]
  public void Report_IgnoresANullException() {
    ErrorDialog.Report(null);
    ErrorDialog.ReportBlocking(null);
  }

  [Fact]
  public void Use_RejectsNullOptions() {
    Assert.Throws<ArgumentNullException>(() => ErrorDialog.Use(null!));
  }

  [Fact]
  public void Report_WritesTheLogAndSkipsTheWindowWhenSuppressed() {
    string? captured = null;
    ErrorDialog.Use(new DesktopErrorOptions {
      Capture = error => {
        captured = error.Message;

        return "saved " + error.Message;
      },
      SuppressWindow = () => true
    });

    ErrorDialog.Report(new InvalidOperationException("boom"));

    Assert.Equal("boom", captured);
  }
}
