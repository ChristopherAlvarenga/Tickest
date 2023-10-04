using System.ComponentModel.DataAnnotations;

namespace Tickest.Models
{
    public class Cargo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string Nome_Cargo { get; set; }

        [Required]
        public required bool Gerenciador { get; set; }

        public required ICollection<Usuario> Usuarios { get; set; }
    }
}
