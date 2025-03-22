using System.ComponentModel.DataAnnotations;
using Core.Bus;
using Core.Controller;
using Core.Notifications;
using DataAccess.Storage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace System.Storage;

[Route("api/storage")]
// [Authorize]
public class StorageController : ApiController
{
    private readonly IStorageService _storageService;
    private readonly DomainNotificationHandler _notifications;

    public StorageController(
        IStorageService minioService,
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler bus) : base(notifications, bus)
    {
        // tenantId = Request.Headers.Where(d => d.Key == "TenantId").Select(d => d.Value).FirstOrDefault();

        _storageService = minioService;
        _notifications = (DomainNotificationHandler)notifications;
    }

    [AllowAnonymous]
    [RedisCache(cacheKeyPrefix: "image", cacheEnabled: true)]
    [Route("upload")]
    [HttpPost]
    public async Task<IActionResult> Upload([Required] IFormFile file,
        CancellationToken cancellation)
    {
        return Response(await _storageService.UploadObjectAsync(file, cancellation));
    }

    [AllowAnonymous]
    [RedisCache(expiryInMinutes:5, cacheKeyPrefix: "image", cacheEnabled: true)]
    [HttpGet("{id}")]
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
    public async Task<IActionResult> Download( [FromRoute] string id,
        CancellationToken cancellation, [FromQuery] bool? isStreamVideo = false)
    {
        if (!(isStreamVideo ?? false))
        {
            var result = await _storageService.DownloadObjectAsync(id.ToString(), cancellation);
            if (result is null)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = _notifications.GetNotifications().Select(n => n.Value)
                });
            }

            return File(result.Data, result.FileStat.ContentType);
        }

        else
        {
            try
            {
                var videoStream = await _storageService.GetPresignedObjectAsync(id, cancellation);
                return Redirect(videoStream);
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., file not found)
                return BadRequest($"Error streaming video: {ex.Message}");
            }
        }
        // try
        // {
        //     var videoStream = await _storageService.GetPresignedObjectAsync(id, cancellation, tenantId);
        //     return Redirect(videoStream);
        // }
        // catch (Exception ex)
        // {
        //     // Handle exceptions (e.g., file not found)
        //     return BadRequest($"Error streaming video: {ex.Message}");
        // }
    }


    [AllowAnonymous]
    [HttpDelete("{objectName}")]
    public async Task<IActionResult> Remove(string objectName, CancellationToken cancellation)
    {
        await _storageService.RemoveObjectsAsync(objectName, cancellation);
        return Response();
    }


}