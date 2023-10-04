using Microsoft.EntityFrameworkCore;

namespace Tickest.Models
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options)
        { 
            this.Database.EnsureCreated();
        }

        public DbSet<Anexo> Anexos { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Notificacoes> Notificacoes { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioTicket> UsuarioTickets { get; set; }
    }
}