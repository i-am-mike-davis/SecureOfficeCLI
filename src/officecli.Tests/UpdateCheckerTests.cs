using System;
using System.IO;

namespace OfficeCli.Core.Tests;

public class UpdateCheckerTests
{
    [Fact]
    public void CheckInBackground_HasHardenedEarlyReturn_BeforeDangerousCode()
    {
        // Security requirement: F-001 must be neutralized by an early return
        // at the top of CheckInBackground(), before any network or download code.
        var sourcePath = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(typeof(UpdateCheckerTests).Assembly.Location)!,
            "..", "..", "..", "..", "officecli", "Core", "UpdateChecker.cs"));

        var source = File.ReadAllText(sourcePath);

        // 1. The hardened message must exist
        Assert.Contains("self-update disabled (hardened)", source);

        // 2. The hardened return must appear BEFORE dangerous code
        var hardenedIndex = source.IndexOf("self-update disabled (hardened)");
        var spawnIndex = source.IndexOf("SpawnRefreshProcess");
        var downloadIndex = source.IndexOf("GetStreamAsync");
        var applyPendingIndex = source.IndexOf("ApplyPendingUpdate");

        Assert.True(hardenedIndex < spawnIndex,
            "Hardened return must appear before SpawnRefreshProcess (F-001)");
        Assert.True(hardenedIndex < downloadIndex,
            "Hardened return must appear before GetStreamAsync download (F-001)");
        Assert.True(hardenedIndex < applyPendingIndex,
            "Hardened return must appear before ApplyPendingUpdate (F-001)");
    }

    [Fact]
    public void AppConfig_AutoUpdateDefaultsToFalse()
    {
        // Security requirement: new installations must start hardened.
        // F-001: self-update download-and-execute pattern must be disabled by default.
        // Test via source inspection (no runtime coupling needed).
        var sourcePath = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(typeof(UpdateCheckerTests).Assembly.Location)!,
            "..", "..", "..", "..", "officecli", "Core", "UpdateChecker.cs"));

        var source = File.ReadAllText(sourcePath);

        // Assert: AutoUpdate property defaults to false (not true)
        Assert.Contains("bool AutoUpdate { get; set; } = false;", source,
            StringComparison.Ordinal);
    }
}
