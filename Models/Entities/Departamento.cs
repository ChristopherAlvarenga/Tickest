using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tickest.Models.Entities
{
    public class Departamento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Nome { get; set; }

        public int ResponsavelId { get; set; }

        public ICollection<Usuario> Usuarios { get; set; }

        public ICollection<Cargo> Cargos { get; set; }

        public ICollection<Area> Areas { get; set; }
    }
}
