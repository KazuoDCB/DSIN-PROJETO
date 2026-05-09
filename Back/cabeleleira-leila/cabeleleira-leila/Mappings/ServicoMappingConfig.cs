using cabeleleira_leila.DTO;
using cabeleleira_leila.Models;
using Mapster;

namespace cabeleleira_leila.Mappings;

public class ServicoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Servico, ServicoResponseDto>();
    }
}
