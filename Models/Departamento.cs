using System.ComponentModel.DataAnnotations;

namespace Tickest.Models
{
    public class Departamento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome_Departamento { get; set; }

        public ICollection<Area> Areas { get; set; }
    }
}
