using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Umbraco.Automate.Core.Settings;

namespace SA.Automate.QRcode.Actions;

/// <summary>
/// Defines the configurable settings for the Save QR Code to Media action in Umbraco Automate.
/// </summary>
public class SaveQrCodeToMediaSettings
{
    /// <summary>The QR code content to save as media. Required.</summary>
    [Required(ErrorMessage = "Value is required.")]
    [Field(
        Label = "Value",
        Description = "The QR code content to save as media - typically bound from the Generate QR Code action's QrCode output. Accepts raw base64 PNG or SVG markup.",
        SupportsBindings = true,
        SortOrder = 1)]
    public string Value { get; set; } = string.Empty;

    /// <summary>The folder to save the media item in. Optional; leave unset to use the Media root.</summary>
    [Field(
        Label = "Media Folder",
        Description = "The folder to save the media item in. Leave unset to save at the root of the Media library.",
        EditorUiAlias = "Umb.PropertyEditorUi.MediaPicker",
        EditorConfig = """[{ "alias": "multiple", "value": false }, { "alias": "validationLimit", "value": { "min": 0, "max": 1 } }, { "alias": "filter", "value": "f38bd2d7-65d0-48e6-95dc-87ce06ec2d3d" }]""",
        SortOrder = 2)]
    public List<MediaPickerValue>? MediaFolder { get; set; }

    /// <summary>The name for the media item, without extension. Optional; auto-generated if unset.</summary>
    [Field(
        Label = "File Name",
        Description = "The name for the media item, without extension (the correct extension is added automatically). Leave unset to auto-generate one. Supports bindings.",
        SupportsBindings = true,
        SortOrder = 3)]
    public string? FileName { get; set; }
}

/// <summary>
/// A single selection from the Media Picker's stored value — a JSON array even when restricted to
/// one item, e.g. <c>[{"mediaKey":"...", ...}]</c>. Only the picked item's key is needed here.
/// </summary>
public sealed record MediaPickerValue([property: JsonPropertyName("mediaKey")] Guid? MediaKey);
