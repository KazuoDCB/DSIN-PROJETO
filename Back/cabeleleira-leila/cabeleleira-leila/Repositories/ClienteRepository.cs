using cabeleleira_leila.DataBase;
using cabeleleira_leila.Interfaces;
using cabeleleira_leila.Models;
using Microsoft.EntityFrameworkCore;

namespace cabeleleira_leila.Repositories;

public class ClienteRepository : BaseRepository<Cliente, long>, IClienteRepository
{
    public ClienteRepository(Cabeleleira_LeilaDbContext context) : base(context)
    {
    }

    public Cliente? GetByEmail(string email)
    {
        return Context
            .Set<Cliente>()
            .AsNoTracking()
            .FirstOrDefault(cliente => cliente.Email == email);
    }

    public async Task<bool> EmailExistsAsync(string email, long? ignoredId, CancellationToken cancellationToken = default)
    {
        return await Context
            .Set<Cliente>()
            .AnyAsync(cliente => cliente.Email == email && cliente.Id != ignoredId, cancellationToken);
    }
}
