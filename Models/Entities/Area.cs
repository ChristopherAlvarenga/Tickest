using System.ComponentModel.DataAnnotations;

namespace Tickest.Models.Entities
{
    public class Area
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Nome { get; set; }

        public int DepartamentoId { get; set; }
        public virtual Departamento Departamento { get; set; }
    }
}
