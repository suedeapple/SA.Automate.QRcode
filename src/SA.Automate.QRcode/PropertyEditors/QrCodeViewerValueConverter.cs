using System.Text.Json;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace SA.Automate.QRcode.PropertyEditors;

/// <summary>
/// Converts a QR Code Viewer property's stored value — a raw code string, or the composite JSON
/// payload from Generate QR Code's <c>QrCodeViewerValue</c> output — into a strongly-typed
/// <see cref="QrCodeValue"/> for Razor views.
/// </summary>
public sealed class QrCodeViewerValueConverter : PropertyValueConverterBase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public override bool IsConverter(IPublishedPropertyType propertyType) =>
        propertyType.EditorAlias == "SA.QrCodeViewer";

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(QrCodeValue);

    public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) =>
        PropertyCacheLevel.Element;

    public override object? ConvertIntermediateToObject(
        IPublishedElement owner,
        IPublishedPropertyType propertyType,
        PropertyCacheLevel referenceCacheLevel,
        object? inter,
        bool preview)
    {
        var raw = (inter as string)?.Trim();
        if (string.IsNullOrEmpty(raw))
            return null;

        if (raw.StartsWith('{'))
        {
            try
            {
                var payload = JsonSerializer.Deserialize<QrCodeViewerPayload>(raw, JsonOptions);
                if (!string.IsNullOrEmpty(payload?.QrCode))
                    return new QrCodeValue(payload.QrCode, payload.Value);
            }
            catch (JsonException)
            {
                // not the composite payload after all — fall through to the raw-code path
            }
        }

        return new QrCodeValue(raw, Value: null);
    }

    private sealed record QrCodeViewerPayload(string? Value, string? QrCode);
}
