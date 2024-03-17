using SixLabors.ImageSharp;

namespace portal_backend.Services;

public class ImageService
{
    private readonly BlobService _blobService;
    private const long MaxFileSizeBytes = 2 * 1024 * 1024;
    private const int MaxWidthHeight = 2000;
    
    public ImageService(BlobService blobService)
    {
        _blobService = blobService;
    }
    public async Task<string> UploadProfilePhoto(IFormFile? photoFile, string photoFileExtention)
    {
        if (photoFile is null)
        {
            return "";
        }
        
        if (photoFile.Length is > MaxFileSizeBytes or <= 0)
        {
            throw new Exception();
        }

        photoFileExtention = photoFileExtention.Replace("image/", ".");
        var fileExtension = photoFileExtention;
        
        if (!(fileExtension.Equals(".png", StringComparison.OrdinalIgnoreCase) || 
              fileExtension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
              fileExtension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase)))
        {
            throw new Exception();
        }
        
        using (var memoryStream = new MemoryStream())
        {
            await photoFile.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            
            using (var image = Image.Load(memoryStream))
            {
                if (image.Width != image.Height || image.Width > MaxWidthHeight)
                {
                    throw new Exception();
                }
            }
        }

        return await _blobService.UploadFile(photoFile, photoFileExtention);
    }
}