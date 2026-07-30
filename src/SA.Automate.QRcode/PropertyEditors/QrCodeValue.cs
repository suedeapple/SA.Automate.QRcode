namespace SA.Automate.QRcode.PropertyEditors;

/// <summary>
/// The strongly-typed value of a QR Code Viewer property, produced by
/// <see cref="QrCodeViewerValueConverter"/> for use in Razor views, e.g.
/// <c>Model.Value&lt;QrCodeValue&gt;("propertyAlias")</c>.
/// </summary>
/// <param name="QrCode">The QR code content: a raw base64 PNG string, a data URI, or SVG markup.</param>
/// <param name="Value">
/// The value that was encoded in the QR code, if known — only present when the property was
/// populated from Generate QR Code's <c>QrCodeViewerValue</c> output rather than <c>QrCode</c>.
/// </param>
public sealed record QrCodeValue(string QrCode, string? Value);
