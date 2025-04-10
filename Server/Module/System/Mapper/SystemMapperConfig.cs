namespace Lottery.Mapper;

public static class SystemMapperConfig
{
    public static Type[] RegisterMappings()
    {
        return new[]    
        {
            typeof(DomainToViewMappingProfile),
            typeof(ViewToDomainMappingProfile),
        };
    }
}