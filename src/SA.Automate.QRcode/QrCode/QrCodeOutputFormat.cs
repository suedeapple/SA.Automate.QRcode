namespace SA.Automate.QRcode.QrCode;

/// <summary>
/// The formats a generated QR code can be returned in.
/// </summary>
public enum QrCodeOutputFormat
{
    /// <summary>A PNG image encoded as a <c>data:image/png;base64,...</c> URI.</summary>
    PngDataUri,

    /// <summary>The raw base64-encoded PNG bytes, without a data URI prefix.</summary>
    RawBase64Png,

    /// <summary>Inline SVG markup.</summary>
    Svg,
}
