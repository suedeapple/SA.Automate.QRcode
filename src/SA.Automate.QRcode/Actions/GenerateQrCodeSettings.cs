using System.ComponentModel.DataAnnotations;
using Umbraco.Automate.Core.Settings;

namespace SA.Automate.QRcode.Actions;

/// <summary>
/// Defines the configurable settings for the Generate QR Code action in Umbraco Automate.
/// </summary>
public class GenerateQrCodeSettings
{
    /// <summary>The value to encode as a QR code. Required.</summary>
    [Required(ErrorMessage = "Value is required.")]
    [StringLength(2000, ErrorMessage = "Value exceeds the maximum length of 2000 characters.")]
    [Field(
        Label = "Value",
        Description = "The value to encode as a QR code, up to 2000 characters. Supports bindings.",
        SupportsBindings = true,
        SortOrder = 1)]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// The output format to render the QR code in. Optional. Stored as a string so the dropdown
    /// picker round-trips cleanly — parsed into <see cref="QrCode.QrCodeOutputFormat"/> at
    /// execute time.
    /// </summary>
    [Field(
        Label = "Output Format",
        Description = "PngDataUri, RawBase64Png, or Svg. Defaults to PNG Data URI if left unset.",
        EditorUiAlias = "Umb.PropertyEditorUi.Dropdown",
        EditorConfig = """[{ "alias": "items", "value": ["PngDataUri", "RawBase64Png", "Svg"] }]""",
        SortOrder = 2)]
    public string? OutputFormat { get; set; }

    /// <summary>The size of each QR module in pixels. Optional; only applies to PNG output.</summary>
    [Field(
        Label = "Size (pixels per module)",
        Description = "The size of each QR module in pixels, from 1 to 50. Only applies to PNG output. Defaults to 20.",
        EditorUiAlias = "Umb.PropertyEditorUi.Integer",
        EditorConfig = """[{ "alias": "min", "value": 1 }, { "alias": "max", "value": 50 }]""",
        SortOrder = 3)]
    public int? PixelsPerModule { get; set; } = 20;

    /// <summary>
    /// The error correction level. Optional. Stored as a string — parsed into
    /// <see cref="QRCoder.QRCodeGenerator.ECCLevel"/> at execute time.
    /// </summary>
    [Field(
        Label = "Error Correction Level",
        Description = "L, M, Q, or H. Higher levels tolerate more damage/obstruction but produce denser codes. Defaults to Q.",
        EditorUiAlias = "Umb.PropertyEditorUi.Dropdown",
        EditorConfig = """[{ "alias": "items", "value": ["L", "M", "Q", "H"] }]""",
        SortOrder = 4)]
    public string? ErrorCorrectionLevel { get; set; }

    /// <summary>The color of the dark modules, as a hex string. Optional.</summary>
    [Field(
        Label = "Dark Color",
        Description = "The color of the dark modules, as a hex string, e.g. #000000. Defaults to black.",
        EditorUiAlias = "Umb.PropertyEditorUi.EyeDropper",
        EditorConfig = """[{ "alias": "showAlpha", "value": false }]""",
        SortOrder = 5)]
    public string? DarkColor { get; set; } = "#000000";

    /// <summary>The color of the light modules, as a hex string. Optional.</summary>
    [Field(
        Label = "Light Color",
        Description = "The color of the light modules, as a hex string, e.g. #FFFFFF. Defaults to white.",
        EditorUiAlias = "Umb.PropertyEditorUi.EyeDropper",
        EditorConfig = """[{ "alias": "showAlpha", "value": false }]""",
        SortOrder = 6)]
    public string? LightColor { get; set; } = "#FFFFFF";

    /// <summary>Whether to draw the standard quiet zone (padding) around the QR code. Optional.</summary>
    [Field(
        Label = "Include Quiet Zone",
        Description = "Draws the standard padding around the QR code, which most cameras need to scan it reliably. Turn off if the code will be embedded somewhere that already provides its own framing.",
        EditorUiAlias = "Umb.PropertyEditorUi.Toggle",
        SortOrder = 7)]
    public bool IncludeQuietZone { get; set; } = true;
}
