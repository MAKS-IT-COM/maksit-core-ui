# MaksIT.Core desktop UI

![Line Coverage](https://img.shields.io/badge/Line%20Coverage-36.7%25-yellow)
![Branch Coverage](https://img.shields.io/badge/Branch%20Coverage-38.1%25-yellow)
![Method Coverage](https://img.shields.io/badge/Method%20Coverage-52.3%25-yellowgreen)

Shared .NET 10 libraries for MaksIT desktop apps. Install from NuGet. Do not reference this folder from another repository.

```bash
dotnet add package MaksIT.Core.Desktop
dotnet add package MaksIT.Core.UI
```

| Package | Role |
|---------|------|
| [MaksIT.Core.Desktop](src/MaksIT.Core.Desktop/MaksIT.Core.Desktop.csproj) | Crash reports, What's New parsing, screenshot-tour flags. No UI toolkit. |
| [MaksIT.Core.UI](src/MaksIT.Core.UI/MaksIT.Core.UI.csproj) | Avalonia windows: About, What's New, logs, crash dialog, and screenshot capture. Depends on MaksIT.Core.Desktop. |

| | |
|--|--|
| Tests / coverage badges | `utils\Invoke-TestEngine.bat` |
| Release (pack, NuGet, GitHub) | `utils\Invoke-ReleasePackage.bat` |
| Contributing | [CONTRIBUTING.md](CONTRIBUTING.md) |
| Changelog | [CHANGELOG.md](CHANGELOG.md) |
| License | [LICENSE.md](LICENSE.md) |

Version is `<Version>` in each library project. Keep the two versions the same. The release reads the version from MaksIT.Core.Desktop, then packs and pushes each package. MaksIT.Core.UI depends on MaksIT.Core.Desktop at that version.
