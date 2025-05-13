namespace AACS.Mapper;

public static class AACSMapperConfig
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