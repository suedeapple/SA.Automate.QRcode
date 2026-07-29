namespace SA.Automate.QRcode.Actions;

/// <summary>
/// Output produced by the Generate QR Code action.
/// </summary>
public sealed class GenerateQrCodeOutput
{
    /// <summary>Gets the value that was encoded in the QR code.</summary>
    public string? Value { get; init; }

    /// <summary>Gets the output format the QR code was rendered in, e.g. "RawBase64Png".</summary>
    public string? OutputFormat { get; init; }

    /// <summary>Gets the generated QR code content: a raw base64 string, a data URI, or SVG markup, depending on the output format.</summary>
    public string? QrCode { get; init; }

    /// <summary>Gets the MIME type of the generated QR code, e.g. "image/png" or "image/svg+xml".</summary>
    public string? MimeType { get; init; }
}
