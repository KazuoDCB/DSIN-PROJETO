using cabeleleira_leila.Models;
using Microsoft.EntityFrameworkCore;

namespace cabeleleira_leila.DataBase
{
    public class Cabeleleira_LeilaDbContext : DbContext
    {
        public Cabeleleira_LeilaDbContext(DbContextOptions<Cabeleleira_LeilaDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Scheduling> Schedulings => Set<Scheduling>();
        public DbSet<Servico> Servicos => Set<Servico>();

        public override int SaveChanges()
        {
            ApplyAuditDates();

            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureCliente(modelBuilder);
            ConfigureServico(modelBuilder);
            ConfigureScheduling(modelBuilder);
        }

        private static void ConfigureCliente(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<User>();

            entity.ToTable("Users");
            entity.HasKey(User => User.Id);
            entity.Property(User => User.Name).HasMaxLength(150).IsRequired();
            entity.Property(User => User.Number).HasMaxLength(20).IsRequired();
            entity.Property(User => User.Email).HasMaxLength(255).IsRequired();
            entity.Property(User => User.PasswordHash).HasMaxLength(512).IsRequired();
            entity.Property(User => User.Role).IsRequired();
            entity.HasIndex(User => User.Email).IsUnique();
            entity
                .HasMany(User => User.Schedulings)
                .WithOne(scheduling => scheduling.User)
                .HasForeignKey(scheduling => scheduling.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureServico(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Servico>();

            entity.ToTable("Servicos");
            entity.HasKey(servico => servico.Id);
            entity.Property(servico => servico.Name).HasMaxLength(120).IsRequired();
            entity.Property(servico => servico.Description).HasMaxLength(500).IsRequired();
            entity.Property(servico => servico.Price).HasPrecision(10, 2);
            entity.Property(servico => servico.Duration).IsRequired();
            entity.HasIndex(servico => servico.Name).IsUnique();
        }

        private static void ConfigureScheduling(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Scheduling>();

            entity.ToTable("Schedulings");
            entity.HasKey(scheduling => scheduling.Id);
            entity.Property(scheduling => scheduling.DataHora).IsRequired();
            entity.HasIndex(scheduling => scheduling.ClienteId);
            entity.HasIndex(scheduling => scheduling.DataHora).IsUnique();
            entity
                .HasMany(scheduling => scheduling.Servicos)
                .WithMany(servico => servico.Schedulings)
                .UsingEntity<Dictionary<string, object>>(
                    "SchedulingServico",
                    right => right
                        .HasOne<Servico>()
                        .WithMany()
                        .HasForeignKey("ServicoId")
                        .OnDelete(DeleteBehavior.Cascade),
                    left => left
                        .HasOne<Scheduling>()
                        .WithMany()
                        .HasForeignKey("SchedulingId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join =>
                    {
                        join.ToTable("SchedulingServicos");
                        join.HasKey("SchedulingId", "ServicoId");
                    });
        }

        private void ApplyAuditDates()
        {
            var entries = ChangeTracker
                .Entries<BaseModel>()
                .Where(entry => entry.State is EntityState.Added or EntityState.Modified);

            var utcNow = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                if (entry.State is EntityState.Added) entry.Entity.MarkAsCreated(utcNow);
                if (entry.State is EntityState.Modified) entry.Entity.MarkAsUpdated(utcNow);
            }
        }
    }
}
