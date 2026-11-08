using System;
using System.IO;

namespace OfficeCli.Tests.Security;

public class UpdateCheckerTests
{
    [Fact]
    [Trait("Category", "Security")]
    [Trait("Component", "UpdateChecker")]
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
    [Trait("Category", "Security")]
    [Trait("Component", "UpdateChecker")]
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

    [Fact]
    [Trait("Category", "Security")]
    [Trait("Component", "UpdateChecker")]
    public void AppConfig_LogDefaultsToTrue()
    {
        // Security requirement: debug logging must be enabled by default
        // so hardened-mode early returns are visible for troubleshooting.
        // Test via source inspection (no runtime coupling needed).
        var sourcePath = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(typeof(UpdateCheckerTests).Assembly.Location)!,
            "..", "..", "..", "..", "officecli", "Core", "UpdateChecker.cs"));

        var source = File.ReadAllText(sourcePath);

        // Assert: Log property defaults to true
        Assert.Contains("bool Log { get; set; } = true;", source,
            StringComparison.Ordinal);
    }

    [Fact]
    [Trait("Category", "Security")]
    [Trait("Component", "UpdateChecker")]
    public void RunRefresh_HasHardenedEarlyReturn()
    {
        // Security requirement: F-001 must be neutralized by an early return
        // at the top of RunRefresh(), before any network or download code.
        var sourcePath = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(typeof(UpdateCheckerTests).Assembly.Location)!,
            "..", "..", "..", "..", "officecli", "Core", "UpdateChecker.cs"));

        var source = File.ReadAllText(sourcePath);

        // Extract the RunRefresh() method body (from method signature to closing brace)
        var runRefreshStart = source.IndexOf("internal static void RunRefresh()");
        Assert.True(runRefreshStart >= 0, "RunRefresh() method must exist");

        // Find the opening brace of RunRefresh()
        var openBrace = source.IndexOf('{', runRefreshStart);
        Assert.True(openBrace >= 0, "RunRefresh() must have an opening brace");

        // Find the matching closing brace (simple brace counting)
        int braceCount = 1;
        int pos = openBrace + 1;
        while (braceCount > 0 && pos < source.Length)
        {
            if (source[pos] == '{') braceCount++;
            else if (source[pos] == '}') braceCount--;
            pos++;
        }
        var runRefreshBody = source.Substring(openBrace, pos - openBrace);

        // 1. The hardened message must exist in RunRefresh()
        Assert.Contains("self-update disabled (hardened)", runRefreshBody,
            StringComparison.Ordinal);

        // 2. The hardened return must appear BEFORE dangerous code in RunRefresh()
        var hardenedIndex = runRefreshBody.IndexOf("self-update disabled (hardened)");
        var spawnIndex = runRefreshBody.IndexOf("SpawnRefreshProcess");
        var downloadIndex = runRefreshBody.IndexOf("GetStreamAsync");
        var applyPendingIndex = runRefreshBody.IndexOf("ApplyPendingUpdate");

        Assert.True(hardenedIndex < spawnIndex,
            "Hardened return must appear before SpawnRefreshProcess (F-001)");
        Assert.True(hardenedIndex < downloadIndex,
            "Hardened return must appear before GetStreamAsync download (F-001)");
        Assert.True(hardenedIndex < applyPendingIndex,
            "Hardened return must appear before ApplyPendingUpdate (F-001)");
    }
}
