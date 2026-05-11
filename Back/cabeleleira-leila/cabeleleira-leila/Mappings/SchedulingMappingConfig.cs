using cabeleleira_leila.DTO;
using cabeleleira_leila.Models;
using Mapster;

namespace cabeleleira_leila.Mappings;

public class SchedulingMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Scheduling, SchedulingResponseDto>()
            .Map(destination => destination.ClienteName, source => source.User.Name)
            .Map(destination => destination.TotalValue, source => source.Servicos.Sum(servico => servico.Price))
            .Map(destination => destination.TotalDuration, source => source.Servicos.Sum(servico => servico.Duration))
            .Map(destination => destination.Servicos, source => source.Servicos.OrderBy(servico => servico.Name));
    }
}
