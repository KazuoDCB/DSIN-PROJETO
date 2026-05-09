using cabeleleira_leila.Models;
using Microsoft.EntityFrameworkCore;

namespace cabeleleira_leila.DataBase
{
    public class Cabeleleira_LeilaDbContext : DbContext
    {
        public Cabeleleira_LeilaDbContext(DbContextOptions<Cabeleleira_LeilaDbContext> options) : base(options)
        {
        }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<scheduling> Schedulings { get; set; }
        public DbSet<Servico> Servicos { get; set; }
    }
}
