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
        public DbSet<Especialidade> Especialidades { get; set; }
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
                    Nome = "Departamento TI",
                    GerenciadorId = 3

                });

            modelBuilder.Entity<Especialidade>().HasData(
                new Especialidade
                {
                    Id = 1,
                    Nome = "Redes",
                    DepartamentoId = 1
                },
                new Especialidade
                {
                    Id = 2,
                    Nome = "Suporte Técnico",
                    DepartamentoId = 1
                },
                new Especialidade
                {
                    Id = 3,
                    Nome = "Segurança",
                    DepartamentoId = 1
                },
                new Especialidade
                {
                    Id = 4,
                    Nome = "Gestão",
                    DepartamentoId = 1
                },
                new Especialidade
                {
                    Id = 5,
                    Nome = "Gerenciamento",
                    DepartamentoId = 1
                },
                new Especialidade
                {
                    Id = 6,
                    Nome = "Business Intelligence",
                    DepartamentoId = 1
                },
                new Especialidade
                {
                    Id = 7,
                    Nome = "Infraestrutura",
                    DepartamentoId = 1
                });
        }

    }
}
