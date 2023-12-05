using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Tickest.Models.Entities
{
    public class Usuario : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        public int CargoId { get; set; }
        public virtual Cargo Cargo { get; set; }

        public int DepartamentoId { get; set; }
        public virtual Departamento Departamento { get; set; }

        public ICollection<UsuarioTicket> UsuarioTickets { get; set; }
    }
}
