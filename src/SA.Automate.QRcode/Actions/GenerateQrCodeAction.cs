using QRCoder;
using SA.Automate.QRcode.QrCode;
using Umbraco.Automate.Core.Actions;

namespace SA.Automate.QRcode.Actions;

/// <summary>
/// Umbraco Automate action that generates a QR code for a value.
/// </summary>
[Action("qrCode.GenerateQrCode", "Generate QR Code",
    Description = "Generates a QR code for a value",
    Group = "QR Code",
    Icon = "icon-barcode",
    ConnectionTypeAlias = "qrCode")]
public class GenerateQrCodeAction : ActionBase<GenerateQrCodeSettings, GenerateQrCodeOutput>
{
    public GenerateQrCodeAction(ActionInfrastructure infrastructure)
        : base(infrastructure)
    {
    }

    /// <summary>
    /// Executes the action by rendering a QR code for the value with QRCoder. Settings are
    /// already validated against their data annotations before this runs. Returns a failed
    /// result on rendering failures.
    /// </summary>
    public override Task<ActionResult> ExecuteAsync(
        ActionContext context,
        CancellationToken cancellationToken)
    {
        var settings = context.GetSettings<GenerateQrCodeSettings>();

        var outputFormat = ParseEnum<QrCodeOutputFormat>(settings.OutputFormat) ?? QrCodeOutputFormat.RawBase64Png;

        var eccLevel = ParseEnum<QRCodeGenerator.ECCLevel>(settings.ErrorCorrectionLevel) ?? QRCodeGenerator.ECCLevel.Q;

        try
        {
            var (content, mimeType) = QrCodeRenderer.Render(
                settings.Value,
                outputFormat,
                settings.PixelsPerModule ?? 20,
                eccLevel,
                settings.DarkColor ?? "#000000",
                settings.LightColor ?? "#FFFFFF",
                settings.IncludeQuietZone);

            return Task.FromResult(Success(new GenerateQrCodeOutput
            {
                Value = settings.Value,
                OutputFormat = outputFormat.ToString(),
                QrCode = content,
                MimeType = mimeType,
            }));
        }
        catch (Exception ex)
        {
            return Task.FromResult(ActionResult.Failed(ex, StepRunErrorCategory.InvalidResponse));
        }
    }

    private static TEnum? ParseEnum<TEnum>(string? value) where TEnum : struct, Enum =>
        Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed) ? parsed : null;
}
