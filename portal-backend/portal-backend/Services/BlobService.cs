using Azure.Storage.Blobs;

namespace portal_backend.Services;

public class BlobService
{
    private readonly BlobContainerClient _containerClient;
    
    public BlobService(IConfiguration configuration)
    {
        var blobServiceClient = new BlobServiceClient(configuration["Blob:Key"]);
        _containerClient = blobServiceClient.GetBlobContainerClient(configuration["Blob:PhotoContainerName"]);
    }

    public async Task<string> UploadFile(IFormFile photo, string extention)
    {
        var blobName = $"{Guid.NewGuid()}{extention}";
        var blobClient = _containerClient.GetBlobClient(blobName);

        await using (var stream = photo.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, true);
        }

        return blobClient.Uri.ToString();
    }
}