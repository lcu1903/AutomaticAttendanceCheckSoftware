using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Minio.DataModel;

namespace System.Storage;

public interface IStorageService : IDomainService
{
    Task<string> UploadObjectAsync(IFormFile file, CancellationToken cancellation);
    Task<ObjectTypeMinio?> DownloadObjectAsync(string id, CancellationToken cancellation);
    Task RemoveObjectsAsync(string objectName, CancellationToken cancellation);
    Task<string> GetPresignedObjectAsync(string id, CancellationToken cancellation);
    Task CleanupUnusedObjectsAsync(CancellationToken cancellationToken);


}