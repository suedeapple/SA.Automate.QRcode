# SA.Automate.QRcode

[![Downloads](https://img.shields.io/nuget/dt/SA.Automate.QRcode?color=cc9900)](https://www.nuget.org/packages/SA.Automate.QRcode/)
[![NuGet](https://img.shields.io/nuget/vpre/SA.Automate.QRcode?color=0273B3)](https://www.nuget.org/packages/SA.Automate.QRcode)
[![GitHub license](https://img.shields.io/github/license/suedeapple/SA.Automate.QRcode?color=8AB803)](https://github.com/suedeapple/SA.Automate.QRcode/blob/main/LICENSE)

QR code generation actions for [Umbraco Automate](https://github.com/umbraco/Umbraco.Automate), powered by [QRCoder](https://github.com/Shane32/QRCoder). Generate a QR code for any value as part of an automation workflow.

## What can this be used for?

This package is useful when you want to generate a QR code inside an Umbraco Automate workflow, for example:

- **Content workflows**: generate a QR code for a page URL whenever content is published, and store it alongside the content.
- **Notifications**: embed a QR code linking to a page in an email or message sent by another action.
- **Print/export flows**: produce a scannable code for a URL, reference number, or other value as part of a larger export or document-generation automation.

The value isn't limited to URLs — anything a QR scanner understands works, e.g. a phone number
(`tel:+441234567890`), an email address (`mailto:someone@example.com`), an SMS
(`sms:+441234567890`), Wi-Fi credentials (`WIFI:S:MyNetwork;T:WPA;P:MyPassword;;`), or just plain
text.

## Installation

```bash
dotnet add package SA.Automate.QRcode
```

No further setup required. The composer registers itself automatically via Umbraco's `IComposer` discovery.

## Connection types

This package registers a single **QR Code** connection type. There's nothing to configure — no
API key or external service, since QR codes are generated locally.

## Setup

### 1. Create the connection in the backoffice

1. Go to **Automate → Connections** and create a new **QR Code** connection.
2. Give the connection a name.
3. Click **Test connection** to verify it's registered correctly.

## Usage

There are two ways to get a generated QR code onto a content item: as the raw string (via the
**QR Code Viewer** property editor) or as an actual Media item (via **Save QR Code to Media**).
**Prefer the text string approach** (`Generate QR Code` → `Update Content Property` → **QR Code
Viewer**) unless you specifically need a Media item — it's lightweight, since it just stores a
string on the content item, with no extra Media item created or file stored on disk.

### Generate QR Code

Add the **Generate QR Code** action to any automation and select the connection to use. Available fields:

| Field | Description |
|---|---|
| Value | The value to encode as a QR code — a URL, `tel:`/`mailto:`/`sms:` link, plain text, or anything else a scanner understands. Supports `${ binding }` expressions. Max 2000 characters. |
| Output Format | Optional. `PngDataUri`, `RawBase64Png`, or `Svg`. Defaults to `PngDataUri` if left unset. |
| Size (pixels per module) | Optional. The size of each QR module in pixels, from 1 to 50. Only applies to PNG output. Defaults to 20. |
| Error Correction Level | Optional. `L`, `M`, `Q`, or `H`. Higher levels tolerate more damage/obstruction but produce denser codes. Defaults to `Q`. |
| Dark Color | Optional. The color of the dark modules, e.g. `#000000`. Defaults to black. |
| Light Color | Optional. The color of the light modules, e.g. `#FFFFFF`. Defaults to white. |
| Include Quiet Zone | Draws the standard padding around the QR code, which most cameras need to scan it reliably. Defaults to on — turn off if the code will be embedded somewhere that already provides its own framing. |

The action outputs the following, which can be referenced via bindings in later workflow steps:

| Output | Description |
|---|---|
| Value | The value that was encoded in the QR code. |
| OutputFormat | The output format the QR code was rendered in, e.g. `PngDataUri`. |
| QrCode | The generated QR code content: a `data:image/png;base64,...` URI, a raw base64 string, or SVG markup, depending on the output format. |
| MimeType | The MIME type of the generated QR code, e.g. `image/png` or `image/svg+xml`. |

### Save QR Code to Media

Add the **Save QR Code to Media** action to save a generated QR code as an actual Media item —
useful for feeding it into a Media Picker property, or anywhere else that needs a real media
reference rather than a raw string. Available fields:

| Field | Description |
|---|---|
| Value | The QR code content to save as media — typically bound from **Generate QR Code**'s `QrCode` output. Accepts a PNG data URI, raw base64 PNG, or SVG markup. |
| Media Folder | Optional. The folder to save the media item in. Leave unset to save at the root of the Media library. |
| File Name | Optional. The name for the media item, without extension — the correct extension is added automatically. Leave unset to auto-generate one. Supports `${ binding }` expressions. |

PNG content is saved as an `Image` media item; SVG content is saved as a `Vector Graphics (SVG)`
media item — Umbraco's built-in media type for SVGs.

The action outputs the following, which can be referenced via bindings in later workflow steps:

| Output | Description |
|---|---|
| MediaId | The numeric Id of the created media item. |
| MediaKey | The Key (GUID) of the created media item. |
| MediaUdi | The media item's UDI, e.g. `umb://media/...`. Bind this directly into a Media Picker property — no formatting needed. |

Typical chain: **Generate QR Code** → **Save QR Code to Media** (bind `QrCode` to `Value`) →
**Update Content Property** on a Media Picker property (bind `MediaUdi`).

## Property editors

This package also registers a **QR Code Viewer** content property editor. It's read-only: it
displays a QR code image on a content item and offers a **Remove** button to clear it, but there's
no text box to type or paste a value into — the value has to be written by a workflow.

### Setup

1. In **Settings → Data Types**, create a new Data Type using the **QR Code Viewer** editor.
2. Add it to a Document Type property.
3. In an Automate workflow, add a **Generate QR Code** action, then an **Update Content Property**
   action bound to its `QrCode` output, targeting that property.

Once the workflow runs and the content is saved, the property displays the generated QR code.
Editors can remove it (and save/publish) to clear it, but can't set a new value directly — that
always goes through the workflow.

## Rendering in views

This package also registers a `<qr-code>` Tag Helper for rendering an already-generated QR code
string (e.g. a QR Code Viewer property's value, or `Generate QR Code`'s `QrCode` output) on the
front end. It doesn't generate anything itself — no QRCoder call, no Automate involved — it just
renders whatever string you give it as an `<img>` or inline `<svg>`, whichever the value actually is.

Register it once in `_ViewImports.cshtml`:

```cshtml
@addTagHelper *, SA.Automate.QRcode
```

Then use it in any view:

```cshtml
<qr-code value="@Model.Value<string>("qrCode")" class="qr-code" width="200" height="200" alt="Scan to view" />
```

`value` and `alt` are the only special attributes — everything else you write on the tag (`class`,
`width`, `height`, `data-*`, or anything else) passes straight through to the rendered element.
Inline SVG has no native `alt` attribute, so when the underlying value is SVG markup, `alt` is
applied as `role="img" aria-label="..."` on the `<svg>` instead — same accessibility intent as an
`<img alt>`, just via the SVG-appropriate mechanism.

## Compatibility

| Package version | Umbraco Automate | Umbraco CMS |
|---|---|---|
| 1.x | 17.x – 18.x | 17.x – 18.x |

## Links

- [Source code](https://github.com/suedeapple/SA.Automate.QRcode)
- [Report an issue](https://github.com/suedeapple/SA.Automate.QRcode/issues)
- [QRCoder documentation](https://github.com/Shane32/QRCoder)
