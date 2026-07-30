using System.Text.Json;
using System.Xml.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SA.Automate.QRcode.PropertyEditors;

namespace SA.Automate.QRcode.Rendering;

/// <summary>
/// Renders an already-generated QR code as an <c>&lt;img&gt;</c> or inline <c>&lt;svg&gt;</c>.
/// <c>value</c> accepts: a raw code string (base64 PNG, a data URI, or SVG markup — e.g. Generate
/// QR Code's <c>QrCode</c> output); the <c>{"value":"...","qrCode":"..."}</c> JSON payload from
/// <c>QrCodeViewerValue</c>; or a <see cref="QrCodeValue"/>, as returned when reading a QR Code
/// Viewer property via <c>Model.Value&lt;QrCodeValue&gt;("propertyAlias")</c>. Only the code is
/// ever rendered — the encoded value (where present) is ignored. Any attribute besides
/// <c>value</c>/<c>alt</c> written on the tag (e.g. <c>class</c>, <c>width</c>, <c>height</c>)
/// passes through untouched.
/// </summary>
[HtmlTargetElement("qr-code")]
public class QrCodeTagHelper : TagHelper
{
    [HtmlAttributeName("value")]
    public object? Value { get; set; }

    [HtmlAttributeName("alt")]
    public string? Alt { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var code = ExtractQrCode(Value);
        if (string.IsNullOrWhiteSpace(code))
        {
            output.SuppressOutput();
            return;
        }

        var trimmed = code.Trim();

        if (trimmed.StartsWith("<svg", StringComparison.OrdinalIgnoreCase))
            RenderSvg(trimmed, output);
        else
            RenderImg(trimmed, output);
    }

    /// <summary>
    /// Extracts the code to render from whichever shape <c>value</c> was bound to: a
    /// <see cref="QrCodeValue"/> straight from the property value converter, or a string — either
    /// a plain code (the common case) or the <c>{"value":"...","qrCode":"..."}</c> payload from
    /// <c>QrCodeViewerValue</c>, which is unwrapped down to just the code. Malformed JSON falls
    /// back to the string unchanged.
    /// </summary>
    private static string? ExtractQrCode(object? value)
    {
        switch (value)
        {
            case null:
                return null;
            case QrCodeValue qrCodeValue:
                return qrCodeValue.QrCode;
            case string { Length: > 0 } text when text.TrimStart().StartsWith('{'):
                try
                {
                    using var document = JsonDocument.Parse(text);
                    if (document.RootElement.TryGetProperty("qrCode", out var qrCode) && qrCode.ValueKind == JsonValueKind.String)
                        return qrCode.GetString() ?? text;
                }
                catch (JsonException)
                {
                    // not JSON after all — treat as a plain code string
                }
                return text;
            case string text:
                return text;
            default:
                return value.ToString();
        }
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
