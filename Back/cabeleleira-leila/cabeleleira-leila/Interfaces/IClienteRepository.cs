using cabeleleira_leila.Models;

namespace cabeleleira_leila.Interfaces;

public interface IClienteRepository : IBaseRepository<Cliente, long>
{
    Cliente? GetByEmail(string email);
    Task<bool> EmailExistsAsync(string email, long? ignoredId, CancellationToken cancellationToken = default);
}
