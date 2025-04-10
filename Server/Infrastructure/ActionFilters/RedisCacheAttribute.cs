using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RedisCacheAttribute : Attribute, IAsyncActionFilter
{
    private readonly int _expiryInMinutes; // Thời gian sống của cache
    private readonly string _cacheKeyPrefix; // Tiền tố cho cache key (ví dụ: "products", "users")
    private readonly string[] _invalidatePatterns; // Các pattern cần xóa khi dữ liệu thay đổi
    private readonly bool _cacheEnabled; // Bật/tắt cache
    private ActionExecutingContext? _context;

    /// <summary>
    /// Constructor cho RedisCacheAttribute
    /// </summary>
    /// <param name="expiryInMinutes">Thời gian sống của cache (phút)</param>
    /// <param name="cacheKeyPrefix">Tiền tố cho cache key</param>
    /// <param name="invalidatePatterns">Danh sách pattern để xóa cache khi dữ liệu thay đổi</param>
    /// <param name="cacheEnabled">Bật/tắt cache (mặc định bật)</param>
    public RedisCacheAttribute(int expiryInMinutes = 10, string cacheKeyPrefix = "", string invalidatePatterns = "", bool cacheEnabled = true)
    {
        _expiryInMinutes = expiryInMinutes;
        _cacheKeyPrefix = cacheKeyPrefix;
        _invalidatePatterns = string.IsNullOrEmpty(invalidatePatterns) ? Array.Empty<string>() : invalidatePatterns.Split(',');
        _cacheEnabled = cacheEnabled;

    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        _context = context;
        
        
        if (!_cacheEnabled) // Bỏ qua nếu cache bị tắt
        {
            await next();
            return;
        }

        var cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();
        string cacheKey = GenerateCacheKey(context.HttpContext, _cacheKeyPrefix);

        // Xử lý cache cho GET
        if (context.HttpContext.Request.Method == "GET")
        {
            byte[]? cachedData = await cache.GetAsync(cacheKey);
            if (cachedData != null)
            {
                context.Result = new ContentResult
                {
                    Content = Encoding.UTF8.GetString(cachedData),
                    ContentType = "application/json",
                    StatusCode = 200
                };
                return;
            }
        }

        // Thực thi action
        var executedContext = await next();

        // Sau khi action thực thi
        if (executedContext.Result is ObjectResult objectResult && objectResult.StatusCode == 200)
        {
            string response = JsonSerializer.Serialize(objectResult.Value);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_expiryInMinutes)
            };

            // Cache response cho GET
            if (context.HttpContext.Request.Method == "GET")
            {
                await cache.SetAsync(cacheKey, Encoding.UTF8.GetBytes(response), cacheOptions);
            }
            // Xóa cache liên quan cho POST/PUT/DELETE
            else if (_invalidatePatterns.Length > 0 && 
                     (context.HttpContext.Request.Method == "POST" || 
                      context.HttpContext.Request.Method == "PUT" || 
                      context.HttpContext.Request.Method == "DELETE"))
            {
                await InvalidateCache(_invalidatePatterns);
            }
        }
    }

    private string GenerateCacheKey(HttpContext context, string prefix)
    {
        // Tạo key dựa trên path, query, và body (nếu có)
        string key = string.IsNullOrEmpty(prefix) ? "cache" : prefix;
        key += $":{context.Request.Path}";

        // Thêm query string nếu có
        if (context.Request.QueryString.HasValue)
        {
            key += $":{context.Request.QueryString}";
        }

        // Thêm body hash nếu là POST/PUT
        if ((
            context.Request.Method == "POST" || 
            context.Request.Method == "PUT" || 
            context.Request.Method == "DELETE" ||
            context.Request.Method == "PATCH"
        ) && context.Request.Body.CanRead)
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            string body = reader.ReadToEndAsync().Result;
            context.Request.Body.Position = 0; // Reset body
            key += $":{HashBody(body)}";
        }

        return key;
    }

    private string HashBody(string body)
    {
        // Tạo hash từ body để tránh key quá dài
        return Convert.ToBase64String(System.Security.Cryptography.SHA256.Create()
            .ComputeHash(Encoding.UTF8.GetBytes(body)));
    }

    private async Task InvalidateCache( string[] patterns)
    {
        var redis = _context.HttpContext.RequestServices.GetRequiredService<IConnectionMultiplexer>();
        var server = redis.GetServer(redis.GetEndPoints()[0]);
        var db = redis.GetDatabase();

        foreach (var pattern in patterns)
        {
            var keys = server.Keys(pattern: $"*:{pattern.Trim()}*").ToArray();
            
            foreach (var key in keys)
            {
                await db.KeyDeleteAsync(key);
            }
        }
    }
}