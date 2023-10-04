using System.ComponentModel.DataAnnotations;

namespace Tickest.Models
{
    public class Cargo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome_Cargo { get; set; }

        [Required]
        public bool Gerenciador { get; set; }

        public ICollection<Usuario> Usuarios { get; set; }
    }
}
