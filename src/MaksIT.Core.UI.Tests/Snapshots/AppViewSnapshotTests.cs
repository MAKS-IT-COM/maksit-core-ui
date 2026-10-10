using MaksIT.Core.UI.Snapshots;

namespace MaksIT.Core.UI.Tests.Snapshots;

public class AppViewSnapshotTests {
  [Theory]
  [InlineData("about", "about")]
  [InlineData("About Window", "About-Window")]
  [InlineData("a_b-c", "a_b-c")]
  [InlineData("café", "caf")]
  [InlineData("---", "view")]
  [InlineData("!!", "view")]
  public void FileToken_KeepsLettersDigitsHyphenAndUnderscore(string id, string expected) {
    Assert.Equal(expected, AppViewSnapshot.FileToken(id));
  }

  [Fact]
  public void Save_ReturnsNullWhenThereIsNoControl() {
    Assert.Null(AppViewSnapshot.Save(null, "unused.png"));
  }

  [Fact]
  public void BoundsIn_RejectsNullArguments() {
    Assert.Throws<ArgumentNullException>(() => AppViewSnapshot.BoundsIn(null!, null!));
  }
}
