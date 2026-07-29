using QRCoder;

namespace SA.Automate.QRcode.QrCode;

/// <summary>
/// Renders QR codes for a piece of text into the output formats supported by the Generate QR
/// Code action.
/// </summary>
public static class QrCodeRenderer
{
    /// <summary>
    /// Generates a QR code for <paramref name="text"/> and renders it as <paramref name="format"/>.
    /// </summary>
    /// <returns>The rendered QR code content, and its MIME type.</returns>
    public static (string Content, string MimeType) Render(
        string text,
        QrCodeOutputFormat format,
        int pixelsPerModule,
        QRCodeGenerator.ECCLevel eccLevel,
        string darkColorHex,
        string lightColorHex,
        bool drawQuietZones)
    {
        using var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(text, eccLevel);

        if (format == QrCodeOutputFormat.Svg)
        {
            var svgQrCode = new SvgQRCode(qrCodeData);
            return (svgQrCode.GetGraphic(pixelsPerModule, darkColorHex, lightColorHex, drawQuietZones), "image/svg+xml");
        }

        // Byte[]-based overload avoids QRCoder's System.Drawing.Color overload, keeping this
        // cross-platform (no libgdiplus dependency at runtime).
        var pngQrCode = new PngByteQRCode(qrCodeData);
        var pngBytes = pngQrCode.GetGraphic(pixelsPerModule, HexToRgba(darkColorHex), HexToRgba(lightColorHex), drawQuietZones);
        var base64 = Convert.ToBase64String(pngBytes);

        return format == QrCodeOutputFormat.PngDataUri
            ? ($"data:image/png;base64,{base64}", "image/png")
            : (base64, "image/png");
    }

    private static byte[] HexToRgba(string hex)
    {
        hex = hex.TrimStart('#');

        var r = Convert.ToByte(hex[..2], 16);
        var g = Convert.ToByte(hex[2..4], 16);
        var b = Convert.ToByte(hex[4..6], 16);
        var a = hex.Length >= 8 ? Convert.ToByte(hex[6..8], 16) : (byte)255;

        return [r, g, b, a];
    }
}
