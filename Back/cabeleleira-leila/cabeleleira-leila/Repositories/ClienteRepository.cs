using cabeleleira_leila.DataBase;
using cabeleleira_leila.Interfaces;
using cabeleleira_leila.Models;
using Microsoft.EntityFrameworkCore;

namespace cabeleleira_leila.Repositories;

public class ClienteRepository : BaseRepository<User, long>, IClienteRepository
{
    public ClienteRepository(Cabeleleira_LeilaDbContext context) : base(context)
    {
    }

    public User? GetByEmail(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return Context
            .Set<User>()
            .AsNoTracking()
            .FirstOrDefault(User => User.Email.ToLower() == normalizedEmail);
    }

    public bool EmailExists(string email, long? ignoredId)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return Context
            .Set<User>()
            .Any(User => User.Email.ToLower() == normalizedEmail && User.Id != ignoredId);
    }
}
