#requires -Version 7.0
#requires -PSEdition Core

<#
.SYNOPSIS
    Registers every packed .nupkg and .snupkg as a GitHub release asset.

.DESCRIPTION
    DotNetPack records only the package it just built. This product packs
    MaksIT.Core.Desktop and MaksIT.Core.UI as two packages. Run this after
    both packs and before GitHub.
#>

if (-not (Get-Command Import-PluginDependency -ErrorAction SilentlyContinue)) {
    $srcDir = Split-Path (Split-Path (Split-Path $PSScriptRoot -Parent) -Parent) -Parent
    $pluginSupportModulePath = Join-Path $srcDir "modules/Engine/PluginSupport.psm1"
    if (Test-Path $pluginSupportModulePath -PathType Leaf) {
        Import-Module $pluginSupportModulePath -Force -Global -ErrorAction Stop
    }
}

function Invoke-Plugin {
    param(
        [Parameter(Mandatory = $true)]
        $Settings
    )

    Import-PluginDependency -ModuleName "Logging" -RequiredCommand "Write-Log"
    Import-PluginDependency -ModuleName "EngineContext" -RequiredCommand "Add-ReleaseAssetPaths"

    $shared = $Settings.context
    $scriptDir = $shared.scriptDir
    $artifactsDirectory = $shared.artifactsDirectory

    if ($Settings.PSObject.Properties['artifactsDir'] -and -not [string]::IsNullOrWhiteSpace([string]$Settings.artifactsDir)) {
        $artifactsDirectory = [System.IO.Path]::GetFullPath((Join-Path $scriptDir ([string]$Settings.artifactsDir)))
    }

    if ([string]::IsNullOrWhiteSpace([string]$artifactsDirectory) -or -not (Test-Path $artifactsDirectory -PathType Container)) {
        throw "AttachNuGetPackages requires artifactsDir, or artifactsDirectory on the shared context."
    }

    $packages = @(
        Get-ChildItem -Path $artifactsDirectory -Filter *.nupkg -File
        Get-ChildItem -Path $artifactsDirectory -Filter *.snupkg -File
    )

    if ($packages.Count -eq 0) {
        throw "AttachNuGetPackages found no .nupkg or .snupkg in $artifactsDirectory"
    }

    Add-ReleaseAssetPaths -Context $shared -Path @($packages.FullName)
    foreach ($package in $packages) {
        Write-Log -Level "OK" -Message "  Release asset: $($package.Name)"
    }
}

Export-ModuleMember -Function Invoke-Plugin
