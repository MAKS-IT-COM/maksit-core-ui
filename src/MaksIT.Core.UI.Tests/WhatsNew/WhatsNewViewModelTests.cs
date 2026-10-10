using MaksIT.Core.Desktop;
using MaksIT.Core.UI.WhatsNew;

namespace MaksIT.Core.UI.Tests.WhatsNew;

public class WhatsNewViewModelTests {
  [Fact]
  public void Notes_StartWithShowAgainEnabled() {
    var notes = new ReleaseNote[] { new("1.0.0", ["Added tests"]) };
    var model = new WhatsNewViewModel(notes);

    Assert.Same(notes, model.Notes);
    Assert.False(model.DoNotShowAgain);
    model.DoNotShowAgain = true;
    Assert.True(model.DoNotShowAgain);
  }
}
