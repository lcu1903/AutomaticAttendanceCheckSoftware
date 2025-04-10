

using Lottery.Mapper;

namespace StartupExtensions;

public static class AutoMapperSetup
{
    public static void AddAutoMapperSetup(this IServiceCollection services)
    {

        services.AddAutoMapper(SystemMapperConfig.RegisterMappings());

    }
}