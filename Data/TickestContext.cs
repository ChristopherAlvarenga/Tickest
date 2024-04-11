using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders.Testing;
using Tickest.Models.Entities;

namespace Tickest.Data
{
    public class TickestContext : IdentityDbContext<Usuario, Role, int>
    {
        public TickestContext(DbContextOptions<TickestContext> options) : base(options)
        {

        }

        public DbSet<Anexo> Anexos { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
		public DbSet<Message> Mensagens { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserToken");


            modelBuilder.Entity<Ticket>()
                .HasOne(p => p.Responsavel)
                .WithMany(p => p.TicketsResponsaveis)
                .HasForeignKey(p => p.ResponsavelId)
                .OnDelete(DeleteBehavior.Restrict);


            //modelBuilder.Entity<Ticket>()
            //    .HasOne(p => p.Solicitante)
            //    .WithMany(p => p.TicketsResponsaveis)
            //    .HasForeignKey(p => p.SolicitanteId)
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Departamento>().HasData(
                new Departamento
                {
                    Id = 1,
                    Nome = "Tecnologia da Informação",
                    ResponsavelId = 3
                },
                new Departamento
                {
                    Id = 2,
                    Nome = "Recursos Humanos",
                    ResponsavelId = 3
                },
                new Departamento
                {
                    Id = 3,
                    Nome = "Almoxarifado",
                    ResponsavelId = 3
                },
                new Departamento
                {
                    Id = 4,
                    Nome = "Gerenciadores",
                    ResponsavelId = 2
                },
                new Departamento
                {
                    Id = 5,
                    Nome = "Contabilidade",
                    ResponsavelId = 3
                },
                new Departamento
                {
                    Id = 6,
                    Nome = "Controladoria",
                    ResponsavelId = 3
                },
                new Departamento
                {
                    Id = 7,
                    Nome = "Suprimentos",
                    ResponsavelId = 3
                },
                new Departamento
                {
                    Id = 8,
                    Nome = "Marketing",
                    ResponsavelId = 3
                },
                new Departamento
                {
                    Id = 9,
                    Nome = "Compras",
                    ResponsavelId = 3
                },
                new Departamento
                {
                    Id = 10,
                    Nome = "Jurídico",
                    ResponsavelId = 3
                },
                new Departamento
                {
                    Id = 11,
                    Nome = "Logística",
                    ResponsavelId = 3
                },
                new Departamento
                {
                    Id = 12,
                    Nome = "Atendimento",
                    ResponsavelId = 3
                });

            modelBuilder.Entity<Area>().HasData(
                new Area
                {
                    Id = 1,
                    Nome = "Redes",
                    DepartamentoId = 1
                },
                new Area
                {
                    Id = 2,
                    Nome = "Suporte Técnico",
                    DepartamentoId = 1
                },
                new Area
                {
                    Id = 3,
                    Nome = "Segurança",
                    DepartamentoId = 1
                },
                new Area
                {
                    Id = 4,
                    Nome = "Gestão",
                    DepartamentoId = 1
                },
                new Area
                {
                    Id = 5,
                    Nome = "Gerenciamento",
                    DepartamentoId = 4
                },
                new Area
                {
                    Id = 6,
                    Nome = "Business Intelligence",
                    DepartamentoId = 1
                },
                new Area
                {
                    Id = 7,
                    Nome = "Infraestrutura",
                    DepartamentoId = 1
                });
        }

    }
}
