using System.ComponentModel.DataAnnotations;

namespace Tickest.Models.Entities
{
    public class Area
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string Nome_Area { get; set; }

        public int DepartamentoId { get; set; }
        public required virtual Departamento Departamento { get; set; }
    }
}
