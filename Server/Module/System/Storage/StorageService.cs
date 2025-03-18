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
    private readonly JwtIssuerOptions _jwtOptions = jwtOptions.Value;
    private readonly IMediatorHandler _bus = bus;
    private readonly ApplicationDbContext _context = context;
    private readonly IMinioClient _minioClient = minioClient;


    public async Task<string> UploadObjectAsync(IFormFile file, CancellationToken cancellation)
    {
        var uuid = Guid.NewGuid().ToString();
        var bucketName = "aacs";
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
        var result = await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(bucketName).WithObject($"{uuid}")
            .WithContentType(file.ContentType)
            .WithStreamData(filestream).WithObjectSize(filestream.Length), cancellation);
        Commit();
        var urlDownload = $"/api/storage/{uuid}";
        return await Task.FromResult(urlDownload);
    }

    public async Task<ObjectTypeMinio?> DownloadObjectAsync(string id, CancellationToken cancellation)
    {
        var bucketName = "aacs";
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
        var bucketName = "aacs";
        var statArgs = new StatObjectArgs()
           .WithBucket(bucketName)
           .WithObject(objectName);

        var stat = await _minioClient.StatObjectAsync(statArgs, cancellation);
        var args = new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName);
        await _minioClient.RemoveObjectAsync(args, cancellation);
        Commit();
    }



    public async Task<StorageRes> GetBucketInfoAsync(string tenantId, CancellationToken cancellation)
    {
        // var bucketName = "aacs";
        // var data = await _context.TenantResources.FindAsync(tenantId);

        // if (data is null)
        // {
        //     //create bucket
        //     var beArgs = new BucketExistsArgs().WithBucket(bucketName);
        //     var found = await _minioClient.BucketExistsAsync(beArgs, cancellation);
        //     if (!found)
        //     {
        //         var mbArgs = new MakeBucketArgs().WithBucket(bucketName);
        //         await _minioClient.MakeBucketAsync(mbArgs, cancellation).ConfigureAwait(false);
        //     }

        //     var tenantResource = new TenantResource
        //     {
        //         TenantId = tenantId,
        //         BucketName = bucketName,
        //         MaxUser = 10,
        //         BucketQuota = 10737418240
        //     };
        //     await _context.TenantResources.AddAsync(tenantResource, cancellation);
        //     var isSuccess = await _context.SaveChangesAsync(cancellation);
        //     if (isSuccess == 1) data = await _context.TenantResources.FindAsync(tenantId);
        //     else return new StorageRes();

        // }
        // var result = new StorageRes
        // {
        //     TenantId = tenantId,
        //     BucketName = bucketName,
        //     BucketObject = data.BucketObject,
        //     BucketQuota = data.BucketQuota ,
        //     BucketUsage = data.ResourceUsage ,
        //     BucketCreationDate = data.CreateDate,
        // };

        // return result;
        return null;
    }

    public async Task<ObjectStat> StatObjectAsync(string id, CancellationToken cancellation)
    {
        var bucketName = "aacs";
        var statArgs = new StatObjectArgs()
            .WithBucket(bucketName)
            .WithObject(id);
        return await _minioClient.StatObjectAsync(statArgs);
    }
    public async Task<string> GetPresignedObjectAsync(string id, CancellationToken cancellation)
    {
        var bucketName = "aacs";
        var args = new PresignedGetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(id).WithExpiry(60 * 60 * 24);
        return await _minioClient.PresignedGetObjectAsync(
                args
            );
    }

}