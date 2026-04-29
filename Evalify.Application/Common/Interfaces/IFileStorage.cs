namespace Evalify.Application.Common.Interfaces;

public interface IFileStorage
{
    /// <summary>
    /// Saves a file stream and returns (filePath, fileUrl).
    /// folder example: "templates" or "papers"
    /// </summary>
    Task<(string FilePath, string FileUrl)> SaveAsync(
        Stream fileStream,
        string fileName,
        string folder,
        CancellationToken ct);

    void Delete(string filePath);
}
