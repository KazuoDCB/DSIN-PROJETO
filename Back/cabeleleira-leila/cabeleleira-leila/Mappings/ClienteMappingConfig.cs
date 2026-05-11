using cabeleleira_leila.DTO;
using cabeleleira_leila.Models;
using Mapster;

namespace cabeleleira_leila.Mappings;

public class ClienteMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, ClienteResponseDto>();
    }
}
