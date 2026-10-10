using CommunityToolkit.Mvvm.ComponentModel;
using MaksIT.Core.Desktop;


namespace MaksIT.Core.UI.WhatsNew;

public sealed partial class WhatsNewViewModel : ObservableObject {
  public WhatsNewViewModel(IReadOnlyList<ReleaseNote> notes) {
    Notes = notes;
  }

  public IReadOnlyList<ReleaseNote> Notes { get; }

  [ObservableProperty]
  private bool doNotShowAgain;
}
