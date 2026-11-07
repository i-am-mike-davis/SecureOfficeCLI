using System;
using System.IO;

namespace OfficeCli.Tests.Security;

public class InstallJsTests
{
    [Fact]
    [Trait("Category", "Security")]
    [Trait("Component", "InstallJs")]
    public void InstallJs_HasEarlyExitBeforeDownloadLogic()
    {
        // Security requirement: F-002 must be neutralized by an early exit
        // at the top of install.js, before any download or network logic.
        var sourcePath = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(typeof(InstallJsTests).Assembly.Location)!,
            "..", "..", "..", "..", "..", "npm", "install.js"));

        var source = File.ReadAllText(sourcePath);

        // 1. The secured-fork message must exist
        Assert.Contains("does not distribute binaries", source);

        // 2. The early exit must appear BEFORE download logic
        var exitIndex = source.IndexOf("process.exit(0)");
        var requireIndex = source.IndexOf("require('./lib/install-binary')");

        Assert.True(exitIndex >= 0,
            "install.js must contain process.exit(0) for secured fork");
        Assert.True(requireIndex >= 0,
            "install.js must still contain require (preserved for rebase)");
        Assert.True(exitIndex < requireIndex,
            "Hardened exit must appear before require('./lib/install-binary') (F-002)");
    }
}
