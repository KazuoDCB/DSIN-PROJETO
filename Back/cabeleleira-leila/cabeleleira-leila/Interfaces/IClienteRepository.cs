using cabeleleira_leila.Models;

namespace cabeleleira_leila.Interfaces;

public interface IClienteRepository : IBaseRepository<User, long>
{
    User? GetByEmail(string email);
    bool EmailExists(string email, long? ignoredId);
}
