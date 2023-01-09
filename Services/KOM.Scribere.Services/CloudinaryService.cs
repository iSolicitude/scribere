namespace KOM.Scribere.Services;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using KOM.Scribere.Data;
using KOM.Scribere.Data.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary cloudinary;

    public CloudinaryService(Cloudinary cloudinary)
    {
        this.cloudinary = cloudinary;
    }

    public async Task<string> UploadMediaAsync(IFormFile media, string fileName, string folder)
    {
        // Check if the file is an image or a video
        if (!media.ContentType.StartsWith("image/") && !media.ContentType.StartsWith("video/"))
        {
            throw new InvalidOperationException("The file must be an image or a video.");
        }

        byte[] destinationData;

        using (var memoryStream = new MemoryStream())
        {
            await media.CopyToAsync(memoryStream);
            destinationData = memoryStream.ToArray();
        }

        UploadResult uploadResult = null;

        using (var memoryStream = new MemoryStream(destinationData))
        {
            RawUploadParams uploadParams = new RawUploadParams
            {
                Folder = folder,
                File = new FileDescription(fileName, memoryStream),
                UseFilename = true,
            };

            // Set the correct content type for the media file
            uploadParams.Type = media.ContentType;

            uploadResult = await this.cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult?.SecureUri.AbsoluteUri;
    }
}