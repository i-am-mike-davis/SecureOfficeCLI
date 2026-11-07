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

        // 2. The throw must appear BEFORE download logic
        var throwIndex = source.IndexOf("does not distribute binaries");
        var downloadIndex = source.IndexOf("assetUrls");
        var fetchToFileIndex = source.IndexOf("fetchToFile");

        Assert.True(throwIndex >= 0,
            "install-binary.js must contain secured-fork guard");
        if (downloadIndex >= 0)
        {
            Assert.True(throwIndex < downloadIndex,
                "Guard must appear before assetUrls download (F-002)");
        }
        if (fetchToFileIndex >= 0)
        {
            Assert.True(throwIndex < fetchToFileIndex,
                "Guard must appear before fetchToFile download (F-002)");
        }
    }
}
