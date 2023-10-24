using System.ComponentModel.DataAnnotations;

namespace Tickest.Models.Entities
{
    public class Cargo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Nome { get; set; }

        public ICollection<Usuario> Usuarios { get; set; }
    }
}
