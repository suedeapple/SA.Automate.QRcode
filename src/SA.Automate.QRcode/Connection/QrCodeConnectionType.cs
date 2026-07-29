using Umbraco.Automate.Core.Connections;

namespace SA.Automate.QRcode.Connection;

/// <summary>
/// Defines the QR Code connection type for Umbraco Automate.
/// </summary>
[ConnectionType("qrCode", "QR Code",
    Description = "Generate QR codes",
    Group = "QR Codes",
    Icon = "icon-barcode")]
public sealed class QrCodeConnectionType : ConnectionTypeBase<QrCodeConnectionSettings>
{
    public QrCodeConnectionType(ConnectionTypeInfrastructure infrastructure)
        : base(infrastructure)
    {
    }

    /// <summary>
    /// There is nothing to validate — QR codes are generated locally with no external service or
    /// configurable settings.
    /// </summary>
    public override Task<ConnectionValidationResult> ValidateAsync(
        object? settings,
        CancellationToken cancellationToken) =>
        Task.FromResult(ConnectionValidationResult.Success("No configuration required."));
}
