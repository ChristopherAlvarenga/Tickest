using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tickest.Models.Entities;

namespace Tickest.Data
{
    public class TickestContext : IdentityDbContext
    {
        private readonly IConfiguration _configuration;
        public TickestContext(
            DbContextOptions<TickestContext> options, 
            IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Anexo> Anexos { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioTicket> UsuarioTickets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cargo>().HasData(
                new Cargo
                {
                    Id = 1,
                    Nome = ""
                });

            modelBuilder.Entity<Departamento>().HasData(
                new Departamento
                {
                    Id = 1,
                    Nome = "",
                    ResponsavelId = 1,
                });

            modelBuilder.Entity<Area>().HasData(
                new Area
                {
                    Id = 1,
                    Nome = "",
                    DepartamentoId = 1
                });

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nome = "Teste",
                    Email = "teste@gmail.com",
                    CargoId = 1,
                    DepartamentoId= 1
                });
        }

    }
}
