using System;
using System.IO;

namespace OfficeCli.Tests.Security;

public class InstallPs1Tests
{
    [Fact]
    [Trait("Category", "Security")]
    [Trait("Component", "InstallPs1")]
    public void InstallPs1_HasEarlyExit()
    {
        // Security requirement: F-003 must be neutralized — install.ps1 must
        // exit before any download or binary execution logic.
        var sourcePath = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(typeof(InstallPs1Tests).Assembly.Location)!,
            "..", "..", "..", "..", "..", "install.ps1"));

        var source = File.ReadAllText(sourcePath);

        // 1. The secured-fork message must exist
        Assert.Contains("does not distribute binaries", source);

        // 2. The exit must appear BEFORE download logic
        var exitIndex = source.IndexOf("exit 0");
        var invokeWebRequestIndex = source.IndexOf("Invoke-WebRequest");
        var invokeRestMethodIndex = source.IndexOf("Invoke-RestMethod");

        Assert.True(exitIndex >= 0,
            "install.ps1 must contain exit 0 for secured fork");
        Assert.True(invokeWebRequestIndex >= 0,
            "install.ps1 must still contain Invoke-WebRequest (preserved for rebase)");
        Assert.True(exitIndex < invokeWebRequestIndex,
            "Hardened exit must appear before Invoke-WebRequest download (F-003)");
    }
}
