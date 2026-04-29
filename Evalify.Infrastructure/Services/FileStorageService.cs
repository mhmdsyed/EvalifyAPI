using Evalify.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace Evalify.Infrastructure.Services;

public sealed class FileStorageService(IWebHostEnvironment env) : IFileStorage
{
    public async Task<(string FilePath, string FileUrl)> SaveAsync(
        Stream fileStream,
        string fileName,
        string folder,
        CancellationToken ct)
    {
        var uploadsFolder = Path.Combine(env.WebRootPath, "uploads", folder);
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        await using var fs = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(fs, ct);

        var fileUrl = $"/uploads/{folder}/{uniqueFileName}";
        return (filePath, fileUrl);
    }

    public void Delete(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}
