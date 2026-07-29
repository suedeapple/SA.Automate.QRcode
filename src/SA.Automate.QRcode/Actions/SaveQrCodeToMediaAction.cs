using SA.Automate.QRcode.Media;
using Umbraco.Automate.Core.Actions;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace SA.Automate.QRcode.Actions;

/// <summary>
/// Umbraco Automate action that saves a QR code as a Media item.
/// </summary>
[Action("qrCode.SaveQrCodeToMedia", "Save QR Code to Media",
    Description = "Saves a QR code as a Media item",
    Group = "QR Code",
    Icon = "icon-barcode",
    ConnectionTypeAlias = "qrCode")]
public class SaveQrCodeToMediaAction : ActionBase<SaveQrCodeToMediaSettings, SaveQrCodeToMediaOutput>
{
    private readonly IMediaService _mediaService;
    private readonly MediaFileManager _mediaFileManager;
    private readonly MediaUrlGeneratorCollection _mediaUrlGeneratorCollection;
    private readonly IShortStringHelper _shortStringHelper;
    private readonly IContentTypeBaseServiceProvider _contentTypeBaseServiceProvider;

    public SaveQrCodeToMediaAction(
        ActionInfrastructure infrastructure,
        IMediaService mediaService,
        MediaFileManager mediaFileManager,
        MediaUrlGeneratorCollection mediaUrlGeneratorCollection,
        IShortStringHelper shortStringHelper,
        IContentTypeBaseServiceProvider contentTypeBaseServiceProvider)
        : base(infrastructure)
    {
        _mediaService = mediaService;
        _mediaFileManager = mediaFileManager;
        _mediaUrlGeneratorCollection = mediaUrlGeneratorCollection;
        _shortStringHelper = shortStringHelper;
        _contentTypeBaseServiceProvider = contentTypeBaseServiceProvider;
    }

    public override Task<ActionResult> ExecuteAsync(
        ActionContext context,
        CancellationToken cancellationToken)
    {
        var settings = context.GetSettings<SaveQrCodeToMediaSettings>();

        byte[] bytes;
        string extension;
        string mediaTypeAlias;
        try
        {
            (bytes, extension, mediaTypeAlias) = ImageStringDecoder.Decode(settings.Value);
        }
        catch (Exception ex) when (ex is FormatException or ArgumentException)
        {
            return Task.FromResult(ActionResult.Failed(ex, StepRunErrorCategory.Validation));
        }

        var name = string.IsNullOrWhiteSpace(settings.FileName)
            ? $"qr-code-{Guid.NewGuid():N}"
            : settings.FileName;
        var fileName = $"{name}.{extension}";
        var parentId = ResolveParentId(settings.MediaFolder);

        try
        {
            var mediaItem = _mediaService.CreateMedia(fileName, parentId, mediaTypeAlias, Constants.Security.SuperUserId);

            using var stream = new MemoryStream(bytes);
            mediaItem.SetValue(
                _mediaFileManager,
                _mediaUrlGeneratorCollection,
                _shortStringHelper,
                _contentTypeBaseServiceProvider,
                Constants.Conventions.Media.File,
                fileName,
                stream);

            _mediaService.Save(mediaItem, Constants.Security.SuperUserId);

            return Task.FromResult(Success(new SaveQrCodeToMediaOutput
            {
                MediaId = mediaItem.Id,
                MediaKey = mediaItem.Key,
                MediaUdi = Udi.Create(Constants.UdiEntityType.Media, mediaItem.Key).ToString(),
            }));
        }
        catch (Exception ex)
        {
            return Task.FromResult(ActionResult.Failed(ex, StepRunErrorCategory.InvalidResponse));
        }
    }

    private int ResolveParentId(List<MediaPickerValue>? mediaFolder)
    {
        var key = mediaFolder?.FirstOrDefault()?.MediaKey;
        if (key is null)
            return Constants.System.Root;

        // GetById(Guid) was removed in Umbraco 18; GetByIds(IEnumerable<Guid>) is stable across
        // the whole 17.x-18.x range this package supports.
        var parent = _mediaService.GetByIds([key.Value]).FirstOrDefault();
        return parent?.Id ?? Constants.System.Root;
    }
}
