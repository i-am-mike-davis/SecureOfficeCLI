// Copyright 2026 OfficeCLI (https://OfficeCLI.AI)
// SPDX-License-Identifier: Apache-2.0

using System;
using System.IO;
using System.Reflection;

namespace OfficeCli.Core;

/// <summary>
/// Single source of truth for KaTeX asset URLs in generated HTML.
/// F-006: loads KaTeX from embedded bundle (katex-bundle.js) and embedded CSS
/// as data URIs so no network calls are made at runtime.
/// </summary>
internal static class KatexAssets
{
    public const string Version = "0.16.11";

    /// <summary>Load the bundled KaTeX JS from the embedded resource (F-006 bundling).</summary>
    private static string? GetEmbeddedKatexJs()
    {
        var asm = typeof(KatexAssets).Assembly;
        using var stream = asm.GetManifestResourceStream("OfficeCli.Resources.js.katex-bundle.js");
        if (stream == null) return null;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>Load the embedded KaTeX CSS from the assembly resources (F-006).</summary>
    private static string? GetEmbeddedKatexCss()
    {
        var asm = typeof(KatexAssets).Assembly;
        using var stream = asm.GetManifestResourceStream("OfficeCli.Resources.css.katex.min.css");
        if (stream == null) return null;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Data URI for the KaTeX JS bundle — embedded, no network call.
    /// </summary>
    public static string JsDataUri =>
        "data:application/javascript;base64,"
        + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(GetEmbeddedKatexJs() ?? ""));

    /// <summary>
    /// Inline KaTeX JS for &lt;script&gt; tags — embedded, no network call.
    /// </summary>
    public static string JsInline => GetEmbeddedKatexJs() ?? "";

    /// <summary>
    /// Data URI for the KaTeX CSS — embedded, no network call.
    /// </summary>
    public static string CssDataUri =>
        "data:text/css;base64,"
        + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(GetEmbeddedKatexCss() ?? ""));

    /// <summary>
    /// Inline KaTeX CSS for &lt;style&gt; tags — embedded, no network call.
    /// </summary>
    public static string CssInline => GetEmbeddedKatexCss() ?? "";

    /// <summary>
    /// onerror body for the stylesheet &lt;link&gt;: removes the tag (KaTeX js falls back
    /// to plain-text rendering, so a missing css is cosmetic).
    /// </summary>
    public static string CssOnErrorJs => "this.remove()";

    /// <summary>
    /// Final fallback when KaTeX JS fails to load — plain-text math rendering.
    /// </summary>
    public static string JsOnErrorJs(string finalFallbackJs) =>
        finalFallbackJs;
}
