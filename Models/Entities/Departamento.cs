using System.ComponentModel.DataAnnotations;

namespace Tickest.Models.Entities
{
    public class Departamento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string Nome_Departamento { get; set; }

        public required ICollection<Area> Areas { get; set; }
    }
}
