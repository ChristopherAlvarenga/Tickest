using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders.Testing;
using Tickest.Models.Entities;

namespace Tickest.Data
{
    public class TickestContext : IdentityDbContext
    {
        public TickestContext(DbContextOptions<TickestContext> options) : base(options)
        {

        }

        public DbSet<Anexo> Anexos { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Departamento>().HasData(
                new Departamento
                {
                    Id = 1,
                    Nome = "Tecnologia da Informação",
                    ResponsavelId = 3
                });

            modelBuilder.Entity<Departamento>().HasData(
                new Departamento
                {
                    Id = 2,
                    Nome = "Recursos Humanos",
                    ResponsavelId = 3
                });

            modelBuilder.Entity<Departamento>().HasData(
                new Departamento
                {
                    Id = 3,
                    Nome = "Suporte",
                    ResponsavelId = 3
                });

            modelBuilder.Entity<Area>().HasData(
                new Area
                {
                    Id = 1,
                    Nome = "BI",
                    DepartamentoId = 1
                });

            modelBuilder.Entity<Area>().HasData(
                new Area
                {
                    Id = 2,
                    Nome = "Recrutamento",
                    DepartamentoId = 2
                });

            modelBuilder.Entity<Area>().HasData(
                new Area
                {
                    Id = 3,
                    Nome = "Componentes Eletrônicos",
                    DepartamentoId = 3
                });

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nome = "Admin",
                    Email = "admin@localhost"
                },
                new Usuario
                {
                    Id = 2,
                    Nome = "Gerenciador",
                    Email = "gerenciador@localhost"
                },
                new Usuario
                {
                    Id = 3,
                    Nome = "Responsável",
                    Email = "responsavel@localhost"
                },
                new Usuario
                {
                    Id = 4,
                    Nome = "Desenvolvedor",
                    Email = "desenvolvedor@localhost",
                    Cargo = "Analista",
                    DepartamentoId = 1,
                    AreaId = 1
                });
        }

    }
}
