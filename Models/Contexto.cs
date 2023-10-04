using Microsoft.EntityFrameworkCore;

namespace Tickest.Models
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options)
        { 
            this.Database.EnsureCreated();
        }
    }
}