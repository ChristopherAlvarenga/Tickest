using System.ComponentModel.DataAnnotations;

namespace Tickest.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Nome { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression("((?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{6,10})")]
        public required string Senha { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Senha")]
        public required string ConfirmaSenha { get; set; }

        public int CargoId { get; set; }
        public required virtual Cargo Cargo { get; set; }

        public int DepartamentoId { get; set; }
        public required virtual Departamento Departamento { get; set; }

        public required ICollection<UsuarioTicket> UsuarioTickets { get; set; }
    }
}
