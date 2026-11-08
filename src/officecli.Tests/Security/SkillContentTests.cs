using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace OfficeCli.Tests.Security;

public class SkillContentTests
{
    [Fact]
    [Trait("Category", "Security")]
    [Trait("Component", "SkillContent")]
    public void Skills_MustNotReferenceOfficeCliDomain()
    {
        // Security requirement: F-004 — skills must not reference the
        // officecli.ai domain, which is used for binary downloads and
        // skill auto-installation. All skills are embedded locally.
        var skillsRoot = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(typeof(SkillContentTests).Assembly.Location)!,
            "..", "..", "..", "..", "..", "skills"));

        // Scan ALL embedded skill files (csproj embeds skills/**/*)
        var skillFiles = Directory.GetFiles(skillsRoot, "*", SearchOption.AllDirectories);

        var violations = new System.Collections.Generic.List<string>();
        foreach (var file in skillFiles)
        {
            // Skip binary files (images, binaries, etc.) — only check text
            var ext = Path.GetExtension(file).ToLowerInvariant();
            if (new[] { ".pptx", ".docx", ".xlsx", ".png", ".jpg", ".jpeg", ".gif", ".webp", ".glb", ".pdf", ".zip" }.Contains(ext))
                continue;

            var content = File.ReadAllText(file);
            if (content.IndexOf("officecli.ai", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                violations.Add(file);
            }
        }

        Assert.Empty(
            violations.Count > 0
                ? $"Skills must not reference officecli.ai domain: {string.Join(", ", violations.Select(f => Path.GetRelativePath(skillsRoot, f)))}"
                : "");
    }

    [Fact]
    [Trait("Category", "Security")]
    [Trait("Component", "SkillContent")]
    public void Skills_MustNotTeachRemoteCodeExecutionPatterns()
    {
        // Security requirement: F-004 — skills must not teach agents to
        // download and execute remote code via curl, wget, or irm (PowerShell).
        var skillsRoot = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(typeof(SkillContentTests).Assembly.Location)!,
            "..", "..", "..", "..", "..", "skills"));

        // Scan ALL embedded skill files (csproj embeds skills/**/*)
        var skillFiles = Directory.GetFiles(skillsRoot, "*", SearchOption.AllDirectories);

        var violations = new System.Collections.Generic.List<(string File, string Pattern)>();
        foreach (var file in skillFiles)
        {
            // Skip binary files — only check text
            var ext = Path.GetExtension(file).ToLowerInvariant();
            if (new[] { ".pptx", ".docx", ".xlsx", ".png", ".jpg", ".jpeg", ".gif", ".webp", ".glb", ".pdf", ".zip" }.Contains(ext))
                continue;

            var content = File.ReadAllText(file);
            // Match as standalone commands, not substrings of words like "confirm" or "curly"
            var patterns = new[] { @"\bcurl\b", @"\bwget\b", @"\birm\b" };
            foreach (var pattern in patterns)
            {
                if (Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase))
                {
                    // Extract the actual matched token for reporting
                    var match = Regex.Match(content, pattern, RegexOptions.IgnoreCase);
                    violations.Add((file, match.Value));
                }
            }
        }

        Assert.Empty(
            violations.Count > 0
                ? $"Skills must not teach remote code execution patterns: {string.Join(", ", violations.Select(v => $"{Path.GetRelativePath(skillsRoot, v.File)} contains '{v.Pattern}'"))}"
                : "");
    }
}
