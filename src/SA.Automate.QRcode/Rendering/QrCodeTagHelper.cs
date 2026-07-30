using System.Text.Json;
using System.Xml.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SA.Automate.QRcode.Rendering;

/// <summary>
/// Renders an already-generated QR code string (raw base64 PNG, a PNG data URI, or SVG markup —
/// e.g. from the QR Code Viewer property or Generate QR Code's <c>QrCode</c> output) as an
/// <c>&lt;img&gt;</c> or inline <c>&lt;svg&gt;</c>. Also accepts the <c>{"value":"...","qrCode":"..."}</c>
/// JSON payload produced by Generate QR Code's <c>QrCodeViewerValue</c> output — only the code is
/// rendered, the value is ignored. Any attribute besides <c>value</c>/<c>alt</c> written on the
/// tag (e.g. <c>class</c>, <c>width</c>, <c>height</c>) passes through untouched.
/// </summary>
[HtmlTargetElement("qr-code")]
public class QrCodeTagHelper : TagHelper
{
    [HtmlAttributeName("value")]
    public string? Value { get; set; }

    [HtmlAttributeName("alt")]
    public string? Alt { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            output.SuppressOutput();
            return;
        }

        var trimmed = ExtractQrCode(Value.Trim());

        if (trimmed.StartsWith("<svg", StringComparison.OrdinalIgnoreCase))
            RenderSvg(trimmed, output);
        else
            RenderImg(trimmed, output);
    }

    /// <summary>
    /// Unwraps the <c>{"value":"...","qrCode":"..."}</c> payload from <c>QrCodeViewerValue</c>,
    /// if that's what was bound, down to just the code. Falls back to the input unchanged for a
    /// plain code string (the common case) or malformed JSON.
    /// </summary>
    private static string ExtractQrCode(string value)
    {
        if (!value.StartsWith('{'))
            return value;

        try
        {
            using var document = JsonDocument.Parse(value);
            if (document.RootElement.TryGetProperty("qrCode", out var qrCode) && qrCode.ValueKind == JsonValueKind.String)
                return qrCode.GetString() ?? value;
        }
        catch (JsonException)
        {
            // not JSON after all — treat as a plain code string
        }

        return value;
    }

    private void RenderImg(string value, TagHelperOutput output)
    {
        output.TagName = "img";
        output.TagMode = TagMode.SelfClosing;

        var src = value.StartsWith("data:", StringComparison.OrdinalIgnoreCase)
            ? value
            : $"data:image/png;base64,{value}";

        output.Attributes.SetAttribute("src", src);
        if (Alt is not null)
            output.Attributes.SetAttribute("alt", Alt);
        // class/width/height/any other attribute written on <qr-code> is already in
        // output.Attributes (untouched, since only value/alt are bound) and carries over as-is.
    }

    private void RenderSvg(string svgMarkup, TagHelperOutput output)
    {
        var element = XElement.Parse(svgMarkup);

        foreach (var attribute in output.Attributes)
            element.SetAttributeValue(attribute.Name, attribute.Value?.ToString());

        if (Alt is not null)
        {
            element.SetAttributeValue("role", "img");
            element.SetAttributeValue("aria-label", Alt);
        }

        output.TagName = null; // drop the <qr-code> wrapper entirely
        output.Content.SetHtmlContent(element.ToString(SaveOptions.DisableFormatting));
    }
}
