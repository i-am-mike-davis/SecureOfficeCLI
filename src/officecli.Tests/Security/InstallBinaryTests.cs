using System;
using System.IO;

namespace OfficeCli.Tests.Security;

public class InstallBinaryTests
{
    [Fact]
    [Trait("Category", "Security")]
    [Trait("Component", "InstallBinary")]
    public void InstallBinary_EnsureBinaryThrowsInSecuredFork()
    {
        // Security requirement: F-002 must be neutralized — ensureBinary()
        // must throw before any download logic.
        var sourcePath = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(typeof(InstallBinaryTests).Assembly.Location)!,
            "..", "..", "..", "..", "..", "npm", "lib", "install-binary.js"));

        var source = File.ReadAllText(sourcePath);

        // 1. The secured-fork message must exist
        Assert.Contains("does not distribute binaries", source);

        // 2. The guard must appear before the actual download call.
        // await fetchToFile is the call that writes the remote binary to disk.
        var throwIndex = source.IndexOf("does not distribute binaries");
        var awaitFetchIndex = source.IndexOf("await fetchToFile");

        Assert.True(throwIndex >= 0,
            "install-binary.js must contain secured-fork guard");
        Assert.True(awaitFetchIndex >= 0,
            "install-binary.js must still contain await fetchToFile (preserved for rebase)");
        Assert.True(throwIndex < awaitFetchIndex,
            "Guard must appear before await fetchToFile download (F-002)");
    }
}
