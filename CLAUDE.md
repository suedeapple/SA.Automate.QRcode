# SA.Automate.QRcode

QR code generation for [Umbraco Automate](https://github.com/umbraco/Umbraco.Automate), powered by
[QRCoder](https://github.com/Shane32/QRCoder). Ships a "Generate QR Code" Automate action, a
companion connection type (no configurable settings), and a read-only "QR Code Viewer" content
property editor for displaying generated codes on content items.

## Structure

- `src/SA.Automate.QRcode/Actions/` — the Generate QR Code Automate action and its settings/output
- `src/SA.Automate.QRcode/Connection/` — the QR Code connection type
- `src/SA.Automate.QRcode/QrCode/` — QRCoder rendering logic (`QrCodeRenderer`)
- `src/SA.Automate.QRcode/PropertyEditors/` — the QR Code Viewer content property editor (C# `DataEditor`)
- `src/SA.Automate.QRcode/wwwroot/App_Plugins/SA.Automate.QRcode/` — the property editor's backoffice manifest + Lit web component (hand-authored plain JS, no build step — the backoffice serves an import map that resolves `@umbraco-cms/backoffice/*` specifiers at runtime)
- `.github/README.md` — the package README (packed into the NuGet package as `README.md`)

## Build

```bash
dotnet build src/SA.Automate.QRcode/SA.Automate.QRcode.csproj
```

The csproj uses the `Microsoft.NET.Sdk.Razor` SDK (not the plain `Microsoft.NET.Sdk`) so the
`wwwroot` static assets pack into the NuGet package correctly.

To test the property editor UI against a real backoffice, add a project reference from a local
Umbraco site to this csproj — there's no local Umbraco site in this repo to run against directly.

## Git conventions

Do not add a `Co-Authored-By: Claude` (or any Claude/Anthropic) trailer to commit messages in this
repo.
