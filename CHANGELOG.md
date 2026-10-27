# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.1.1] - 2026-10-27

### Changed

- **MaksIT.Core.UI:** Avalonia and Avalonia.Desktop 12.1.2 → 12.1.4.
- Release packs both libraries for the GitHub release; nuget.org publish uses GitHub Actions Trusted Publishing instead of local `DotNetNuGet`.

### Fixed

- Sync RepoUtils release engine (`-InstallOnly` / Helm install helpers) and bump test packages (xunit.v3, coverlet.MTP).

## [0.1.0] - 2026-10-10

### Added

- **MaksIT.Core.Desktop:** crash-report text and log files, What's New markdown parsing, and screenshot-tour command-line flags (`--screenshots`, `--views`, `--settle-ms`).
- **MaksIT.Core.UI:** Avalonia windows for About, What's New, logs, and the crash dialog, plus screenshot capture. Depends on MaksIT.Core.Desktop at the same version.
- Release uses RepoUtils: pack and push each library, then attach both packages to the GitHub release.
