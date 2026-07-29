namespace SA.Automate.QRcode.Actions;

/// <summary>
/// Output produced by the Save QR Code to Media action.
/// </summary>
public sealed class SaveQrCodeToMediaOutput
{
    /// <summary>Gets the numeric Id of the created media item.</summary>
    public int? MediaId { get; init; }

    /// <summary>Gets the Key (GUID) of the created media item.</summary>
    public Guid? MediaKey { get; init; }

    /// <summary>
    /// Gets the media item's UDI (e.g. <c>umb://media/...</c>) — bind this directly into a Media
    /// Picker property via Update Content Property, no formatting needed.
    /// </summary>
    public string? MediaUdi { get; init; }
}
