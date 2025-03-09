using Hangfire;
using Hangfire.Redis.StackExchange;
using Npgsql;
using StackExchange.Redis;

namespace StartupExtensions
{
    public static class HangfireExtension
    {
        public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionStringFormat = configuration.GetConnectionString("DefaultConnection");
            try
            {
                var databaseNeedCreate = new NpgsqlConnection(connectionStringFormat).Database;
                var masterConnection = connectionStringFormat?.Replace(databaseNeedCreate, "postgres");
                using (var sqlConnection = new NpgsqlConnection(masterConnection))
                {
                    sqlConnection.Open();

                    using (var command = new NpgsqlCommand($"create database \"{databaseNeedCreate}\";"))
                    {
                        command.Connection = sqlConnection;
                        command.ExecuteNonQuery();
                        sqlConnection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                // ignored
            }


            services.AddHangfire(config =>
            {
                var isUseDistributedCache = configuration.GetValue<bool>("RedisSettings:IsUseDistributedCache");
                if (!isUseDistributedCache)
                {
                    config.UseInMemoryStorage();
                }
                else
                {
                    config.UseRedisStorage(ConnectionMultiplexer.Connect(configuration.GetValue<string>("RedisSettings:RedisCacheUrl")!), new RedisStorageOptions()
                    {
                        Prefix = configuration.GetValue<string>("RedisSettings:RedisKeyPrefix") + ":"
                    });
                }
            });

            services.AddHangfireServer(config =>
            {
                config.SchedulePollingInterval = TimeSpan.FromMilliseconds(500);
            });


            return services;
        }

        public static IApplicationBuilder UseHangfireDashboard(this IApplicationBuilder app,
            IConfiguration configuration)
        {

            var dashboardOptions = configuration.GetSection("HangfireSettings:Dashboard").Get<DashboardOptions>();

            // dashboardOptions!.Authorization =
            // [
            //     new HangfireCustomBasicAuthenticationFilter
            //     {
            //         User = configuration.GetSection("HangfireSettings:User").Value,
            //         Pass = configuration.GetSection("HangfireSettings:Password").Value
            //     }
            // ];

            return app.UseHangfireDashboard(configuration["HangfireSettings:Route"], dashboardOptions);
        }
    }
}