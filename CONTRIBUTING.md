# Contributing to MaksIT.Core desktop UI

## Development setup

- .NET 10 SDK
- Git
- PowerShell 7+ for RepoUtils

```bash
cd src
dotnet build MaksIT.Core.UI.slnx
```

## Tests

`utils\Invoke-TestEngine.bat` runs DotNetTest, then QualityGate, then CoverageBadges.

Test projects are `src/MaksIT.Core.Desktop.Tests` and `src/MaksIT.Core.UI.Tests`. Tests use Microsoft Testing Platform (`global.json` next to the solution), xunit.v3, and coverlet.MTP. CoverageBadges rewrites the shields.io lines under the README title.

## Commit message format

```text
(type): description
```

Types: `(feature):`, `(bugfix):`, `(refactor):`, `(perf):`, `(test):`, `(docs):`, `(build):`, `(ci):`, `(style):`, `(revert):`, `(chore):`.

Lowercase description. No trailing period.

## Versioning

[Semantic Versioning](https://semver.org/): MAJOR breaking, MINOR additive, PATCH fix.

Keep `<Version>` the same in both library projects. The release tag is `v{version}` and must match MaksIT.Core.Desktop.

## Release

Orchestration is `utils/` from [maksit-repoutils](https://git.maks-it.com/MAKS-IT/maksit-repoutils).

| Entry | Purpose |
|-------|---------|
| `utils\Invoke-TestEngine.bat` | Tests and coverage badges |
| `utils\Invoke-ReleasePackage.bat` | Pack, NuGet, GitHub |
| `utils\Force-AmendTaggedCommit.bat` | Amend the last tagged commit |

1. Bump `<Version>` in both csproj files and update CHANGELOG.md.
2. Commit on `main` and tag `v{version}`.
3. Set RepoUtilsSecrets slots `GitClone` (GitHub token) and `NuGet` (NuGet.org API key).
4. Run `utils\Invoke-ReleasePackage.bat`.

The release packs MaksIT.Core.Desktop, pushes it, then packs and pushes MaksIT.Core.UI. GitHub attaches both packages. `ReleasePublishGuard` skips publish unless the branch is `main` and HEAD is tagged with that version.

## License

Contributions are licensed under `LICENSE.md`.
