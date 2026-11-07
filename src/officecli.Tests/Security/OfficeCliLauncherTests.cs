using System;
using System.IO;

namespace OfficeCli.Tests.Security;

public class OfficeCliLauncherTests
{
    [Fact]
    [Trait("Category", "Security")]
    [Trait("Component", "OfficeCliLauncher")]
    public void OfficeCliLauncher_HasEarlyExitBeforeBinaryDownload()
    {
        // Security requirement: F-002 must be neutralized — main() must exit
        // before any binaryPath or ensureBinary calls.
        var sourcePath = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(typeof(OfficeCliLauncherTests).Assembly.Location)!,
            "..", "..", "..", "..", "..", "npm", "officecli.js"));

        var source = File.ReadAllText(sourcePath);

        // 1. The secured-fork message must exist
        Assert.Contains("does not distribute binaries", source);

        // 2. The early exit must appear before download logic
        var exitIndex = source.IndexOf("process.exit(1)");
        var ensureBinaryIndex = source.IndexOf("ensureBinary");
        var binaryPathIndex = source.IndexOf("binaryPath");

        Assert.True(exitIndex >= 0,
            "officecli.js must contain process.exit(1) for secured fork");
        Assert.True(ensureBinaryIndex >= 0,
            "officecli.js must still contain ensureBinary reference (preserved for rebase)");
        Assert.True(exitIndex < ensureBinaryIndex,
            "Hardened exit must appear before ensureBinary call (F-002)");
    }
}
