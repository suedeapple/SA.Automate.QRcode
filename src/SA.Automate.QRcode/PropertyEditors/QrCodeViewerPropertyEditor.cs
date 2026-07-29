using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;

namespace SA.Automate.QRcode.PropertyEditors;

/// <summary>
/// A read-only content property editor that displays a QR code image. The value is written by an
/// Automate workflow (e.g. Generate QR Code → Update Content Property) — the editor itself only
/// renders it and offers a way to clear it, not to set or type a new one.
/// </summary>
[DataEditor("SA.QrCodeViewer", ValueType = ValueTypes.Text, ValueEditorIsReusable = true)]
public sealed class QrCodeViewerPropertyEditor : DataEditor
{
    public QrCodeViewerPropertyEditor(IDataValueEditorFactory dataValueEditorFactory)
        : base(dataValueEditorFactory)
    {
    }

    protected override IDataValueEditor CreateValueEditor() =>
        DataValueEditorFactory.Create<DataValueEditor>(Attribute!);
}
