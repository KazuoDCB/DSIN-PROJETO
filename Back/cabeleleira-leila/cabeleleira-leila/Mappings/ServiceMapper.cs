using Mapster;
using MapsterMapper;

namespace cabeleleira_leila.Mappings;

public static class ServiceMapper
{
    public static IServiceCollection AddServiceMapper(this IServiceCollection services)
    {
        TypeAdapterConfig config = new();
        config.Scan(typeof(ServiceMapper).Assembly);

        services.AddSingleton(config);
        services.AddSingleton<IMapper>(_ => new Mapper(config));

        return services;
    }
}
