using System.Text;
using Umbraco.Cms.Core;

namespace SA.Automate.QRcode.Media;

/// <summary>
/// Decodes a QR code content string (raw base64 PNG, a PNG data URI, or SVG markup — the formats
/// produced by <see cref="QrCode.QrCodeRenderer"/>) into raw bytes plus the file extension and
/// Umbraco media type alias to save it as.
/// </summary>
public static class ImageStringDecoder
{
    public static (byte[] Bytes, string Extension, string MediaTypeAlias) Decode(string value)
    {
        var trimmed = value.Trim();

        if (trimmed.StartsWith("<svg", StringComparison.OrdinalIgnoreCase))
            return (Encoding.UTF8.GetBytes(trimmed), "svg", Constants.Conventions.MediaTypes.VectorGraphicsAlias);

        if (trimmed.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            var commaIndex = trimmed.IndexOf(',');
            if (commaIndex < 0)
                throw new ArgumentException("Value is a data URI but has no comma-separated payload.");

            var header = trimmed[..commaIndex];
            var payload = trimmed[(commaIndex + 1)..];
            var isSvg = header.Contains("svg", StringComparison.OrdinalIgnoreCase);

            return isSvg
                ? (Convert.FromBase64String(payload), "svg", Constants.Conventions.MediaTypes.VectorGraphicsAlias)
                : (Convert.FromBase64String(payload), "png", Constants.Conventions.MediaTypes.Image);
        }

        // Bare base64 (the RawBase64Png output format has no data: prefix).
        return (Convert.FromBase64String(trimmed), "png", Constants.Conventions.MediaTypes.Image);
    }
}
