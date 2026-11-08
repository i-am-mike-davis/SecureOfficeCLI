// Copyright 2026 OfficeCLI (https://OfficeCLI.AI)
// SPDX-License-Identifier: Apache-2.0

using System;
using System.IO;
using System.Reflection;

namespace OfficeCli.Core;

/// <summary>
/// Single source of truth for Three.js asset URLs in generated HTML.
/// F-006: loads Three.js from embedded bundle (three-bundle.js) as a data URI
/// so no network calls are made at runtime.
/// </summary>
internal static class ThreeAssets
{
    public const string Version = "0.170.0";

    /// <summary>Load the bundled Three.js from the embedded resource (F-006 bundling).</summary>
    private static string? GetEmbeddedThreeJs()
    {
        var asm = typeof(ThreeAssets).Assembly;
        using var stream = asm.GetManifestResourceStream("OfficeCli.Resources.js.three-bundle.js");
        if (stream == null) return null;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Data URI for the Three.js bundle — embedded, no network call.
    /// </summary>
    public static string JsDataUri =>
        "data:application/javascript;base64,"
        + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(GetEmbeddedThreeJs() ?? ""));

    /// <summary>
    /// Inline Three.js for &lt;script&gt; tags — embedded, no network call.
    /// </summary>
    public static string JsInline => GetEmbeddedThreeJs() ?? "";

    /// <summary>
    /// Import map JSON — empty since Three.js is inlined (F-006).
    /// </summary>
    public static string ImportMapJson => "{}";

    /// <summary>
    /// CDN fallback for the core module — not used (F-006: embedded only).
    /// </summary>
    public static string CdnEsmThreeUrl => "";

    /// <summary>
    /// CDN fallback for GLTFLoader — not used (F-006: embedded only).
    /// </summary>
    public static string CdnEsmGltfLoaderUrl => "";
}
