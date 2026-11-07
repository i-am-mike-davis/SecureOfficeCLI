using System;
using System.IO;

namespace OfficeCli.Tests.Security;

public class InstallShTests
{
    [Fact]
    [Trait("Category", "Security")]
    [Trait("Component", "InstallSh")]
    public void InstallSh_HasEarlyExit()
    {
        // Security requirement: F-003 must be neutralized — install.sh must
        // exit before any download or binary execution logic.
        var sourcePath = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(typeof(InstallShTests).Assembly.Location)!,
            "..", "..", "..", "..", "..", "install.sh"));

        var source = File.ReadAllText(sourcePath);

        // 1. The secured-fork message must exist
        Assert.Contains("does not distribute binaries", source);

        // 2. The exit must appear BEFORE download logic
        var exitIndex = source.IndexOf("exit 0");
        var curlIndex = source.IndexOf("curl");
        var wgetIndex = source.IndexOf("wget");

        Assert.True(exitIndex >= 0,
            "install.sh must contain exit 0 for secured fork");
        Assert.True(curlIndex >= 0,
            "install.sh must still contain curl (preserved for rebase)");
        Assert.True(exitIndex < curlIndex,
            "Hardened exit must appear before curl download (F-003)");
    }
}
