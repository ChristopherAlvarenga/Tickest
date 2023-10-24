using System.ComponentModel.DataAnnotations;

namespace Tickest.Models.Entities
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Nome { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        public int CargoId { get; set; }
        public virtual Cargo Cargo { get; set; }

        public int DepartamentoId { get; set; }
        public virtual Departamento Departamento { get; set; }

        public ICollection<UsuarioTicket> UsuarioTickets { get; set; }
    }
}
