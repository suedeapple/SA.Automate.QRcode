using SA.Automate.QRcode.Actions;
using SA.Automate.QRcode.Connection;
using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Connections;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace SA.Automate.QRcode.Configuration;

/// <summary>
/// Registers all QR Code Automate services with the Umbraco dependency injection container.
/// This composer wires up the connection type and available actions.
/// </summary>
public class QrCodeComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        // Register the QR Code connection type so it appears in Umbraco Automate connections
        builder.WithCollectionBuilder<ConnectionTypeCollectionBuilder>()
            .Add<QrCodeConnectionType>();

        // Register the QR Code actions so they are available in Umbraco Automate workflows
        builder.WithCollectionBuilder<ActionCollectionBuilder>()
            .Add<GenerateQrCodeAction>()
            .Add<SaveQrCodeToMediaAction>();
    }
}
