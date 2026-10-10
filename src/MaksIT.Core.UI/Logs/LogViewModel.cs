using CommunityToolkit.Mvvm.ComponentModel;


namespace MaksIT.Core.UI.Logs;

public sealed partial class LogViewModel : ObservableObject {
  private readonly Func<string> _directory;
  private readonly Func<string> _copiedLabel;

  public LogViewModel(Func<string> directory, DesktopLogText text) {
    _directory = directory;
    _copiedLabel = () => text.CopiedLabel;
    CopyLabel = text.CopyLabel;
    CloseLabel = text.CloseLabel;
    Refresh();
  }

  public string CopyLabel { get; }

  public string CloseLabel { get; }

  public IReadOnlyList<string> Files { get; private set; } = [];

  [ObservableProperty]
  private string selectedFile = "";

  [ObservableProperty]
  private string report = "";

  [ObservableProperty]
  private string copyStatus = "";

  public void Refresh() {
    try {
      var directory = _directory();
      Directory.CreateDirectory(directory);
      Files = Directory.GetFiles(directory)
        .OrderByDescending(File.GetLastWriteTimeUtc)
        .Select(Path.GetFileName)
        .Where(name => !string.IsNullOrWhiteSpace(name))
        .Cast<string>()
        .ToList();
    }
    catch {
      Files = [];
    }

    OnPropertyChanged(nameof(Files));

    if (Files.Count == 0) {
      SelectedFile = "";
      Report = "";

      return;
    }

    if (string.IsNullOrWhiteSpace(SelectedFile) || !Files.Contains(SelectedFile, StringComparer.OrdinalIgnoreCase))
      SelectedFile = Files[0];
    else
      LoadSelected();
  }

  partial void OnSelectedFileChanged(string value) =>
    LoadSelected();

  public void MarkCopied() =>
    CopyStatus = _copiedLabel();

  private void LoadSelected() {
    CopyStatus = "";

    if (string.IsNullOrWhiteSpace(SelectedFile)) {
      Report = "";

      return;
    }

    try {
      var path = Path.Combine(_directory(), SelectedFile);
      Report = File.Exists(path) ? File.ReadAllText(path) : "";
    }
    catch (Exception ex) {
      Report = ex.Message;
    }
  }
}
