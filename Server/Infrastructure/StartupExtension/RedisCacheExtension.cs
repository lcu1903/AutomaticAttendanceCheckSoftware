
using StackExchange.Redis;

namespace StartupExtensions
{
    public static class RedisCacheExtension
    {
        private static ConnectionMultiplexer redis;
        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            string redisConnection = configuration.GetValue<string>("RedisSettings:RedisCacheUrl");
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));
            var isUseDistributedCache = configuration.GetValue<bool>("RedisSettings:IsUseDistributedCache");
            if (isUseDistributedCache)
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = configuration.GetValue<string>("RedisSettings:RedisCacheUrl");
                    options.InstanceName = configuration.GetValue<string>("RedisSettings:RedisKeyPrefix") + ":";
                });
            }
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            try
            {
                redis = ConnectionMultiplexer.Connect(redisConnection);
                IDatabase db = redis.GetDatabase();
                // Thử lệnh PING
                string result = db.Execute("PING").ToString();
                Console.WriteLine("Redis response: " + result); // Kết quả mong đợi: PONG
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }


            return services;
        }

        public static IApplicationBuilder UseRedisCache(this IApplicationBuilder app, IConfiguration configuration)
        {
            var isUseDistributedCache = configuration.GetValue<bool>("RedisSettings:IsUseDistributedCache");
            if (isUseDistributedCache)
            {
                app.UseSession();
            }

            return app;
        }
    }

}