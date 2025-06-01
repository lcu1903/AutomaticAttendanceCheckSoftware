using System.Storage;
using Core.Bus;
using Core.Interfaces;
using Core.Notifications;
using DataAccess.Contexts;
using Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Minio;
using Minio.ApiEndpoints;
using Minio.DataModel;
using Minio.DataModel.Args;

namespace DataAccess.Storage;

public class StorageService(
        IMinioClient minioClient,
        IConfiguration configuration,
        IMediatorHandler bus,
        ApplicationDbContext context,
        IUnitOfWork uow,
        INotificationHandler<DomainNotification> notifications,
        IOptions<JwtIssuerOptions> jwtOptions
    )
    : Infrastructure.DomainService.DomainService(notifications, uow, bus), IStorageService
{
    private readonly IMediatorHandler _bus = bus;
    private readonly IMinioClient _minioClient = minioClient;
    private string bucketName = configuration.GetValue<string>("Minio:BucketName");


    public async Task<string> UploadObjectAsync(IFormFile file, CancellationToken cancellation)
    {
        var uuid = Guid.NewGuid().ToString();
        var beArgs = new BucketExistsArgs().WithBucket(bucketName);
        var found = await _minioClient.BucketExistsAsync(beArgs, cancellation);
        if (!found)
        {
            var mbArgs = new MakeBucketArgs().WithBucket(bucketName);
            await _minioClient.MakeBucketAsync(mbArgs, cancellation).ConfigureAwait(false);
        }


        var filestream = new MemoryStream();
        await file.CopyToAsync(filestream, cancellation);
        filestream.Position = 0;
        await _minioClient.PutObjectAsync(new PutObjectArgs()
           .WithBucket(bucketName).WithObject($"{uuid}")
           .WithContentType(file.ContentType)
           .WithStreamData(filestream).WithObjectSize(filestream.Length), cancellation);
        Commit();
        var urlDownload = $"/api/storage/{uuid}";
        return await Task.FromResult(urlDownload);
    }

    public async Task<ObjectTypeMinio?> DownloadObjectAsync(string id, CancellationToken cancellation)
    {

        var nameObject = $"{id}";


        // // var stat = await minioClient.StatObjectAsync(args, cancellation);
        // var stats = await _minioApi.GetObjectstat(bucketName, nameObject);
        // var download = await _minioApi.DownloadObjectAsync(bucketName, nameObject);
        // var destination = new MemoryStream();
        // using (var stream = await download.ReadAsStreamAsync())
        // {
        //     await stream.CopyToAsync(destination);
        // }
        // return await Task.FromResult(new MinioObject
        // {
        //     Data = destination.ToArray(),
        //     ContentType = stats.Headers.ContentType,
        // });
        var args = new StatObjectArgs()
           .WithBucket(bucketName)
           .WithObject(nameObject);

        var stat = await _minioClient.StatObjectAsync(args, cancellation);
        var destination = new MemoryStream();

        var objStatReply = await _minioClient.StatObjectAsync(new StatObjectArgs()
            .WithBucket(bucketName)
            .WithObject(nameObject), cancellation);
        if (objStatReply == null || objStatReply.DeleteMarker)
        {
            await _bus.RaiseEvent(new DomainNotification(string.Empty, "error.notFound"));
            return null;
        }

        await _minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(nameObject)
            .WithCallbackStream(stream => { stream.CopyTo(destination); }
            ), cancellation);
        return await Task.FromResult(new ObjectTypeMinio
        {
            Data = destination.ToArray(),
            FileStat = stat
        });
    }

    public async Task RemoveObjectsAsync(string objectName, CancellationToken cancellation)
    {

        var args = new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName);
        await _minioClient.RemoveObjectAsync(args, cancellation);
        Commit();
    }
    public async Task<string> GetPresignedObjectAsync(string id, CancellationToken cancellation)
    {

        var args = new PresignedGetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(id).WithExpiry(60 * 60 * 24);
        return await _minioClient.PresignedGetObjectAsync(
                args
            );
    }
    public async Task CleanupUnusedObjectsAsync(CancellationToken cancellationToken)
    {
        // Giả sử bạn có bảng chứa ImageUrl và cần lấy hết các image key từ ImageUrl,
        // ví dụ: "/api/storage/{fileKey}" -> lấy fileKey sau dấu '/'
        var imageUrls = new List<string>
        {

        };
        var dbImageUrls = await context.Users   // thay YourTable thành bảng của bạn
            .Select(x => x.ImageUrl)
            .ToListAsync(cancellationToken);
        var attendanceImageUrls = await context.Attendances
            .Select(x => x.ImageUrl)
            .ToListAsync(cancellationToken);
        imageUrls.AddRange(dbImageUrls);
        imageUrls.AddRange(attendanceImageUrls);
        // Trích xuất fileKey từ các URL, giả sử fileKey là phần sau dấu '/'
        var dbFileKeys = imageUrls
            .Where(url => !string.IsNullOrEmpty(url))
            .Select(url => url.Substring(url.LastIndexOf('/') + 1))
            .ToHashSet<string>();

        var unusedFiles = new List<string>();

        // Lấy danh sách object từ Minio
        var listArgs = new ListObjectsArgs()
            .WithBucket(bucketName)
            .WithRecursive(true);

        // Dùng IObservable để nhận danh sách các object
        var observable = _minioClient.ListObjectsAsync(listArgs);

        // Thu thập các file không có trong DB
        var completionSource = new TaskCompletionSource<bool>();
        using (observable.Subscribe(
            item =>
            {
                if (!dbFileKeys.Contains(item.Key))
                {
                    unusedFiles.Add(item.Key);
                }
            },
            ex => completionSource.TrySetException(ex),
            () => completionSource.TrySetResult(true)))
        {
            await completionSource.Task;
        }

        if (unusedFiles.Count > 0)
        {
            var removeObjectsArgs = new RemoveObjectsArgs()
                .WithBucket(bucketName)
                .WithObjects(unusedFiles);

            await _minioClient.RemoveObjectsAsync(removeObjectsArgs, cancellationToken);
        }

    }

}