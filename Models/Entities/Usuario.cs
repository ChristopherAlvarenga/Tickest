using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

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

        public string? Cargo { get; set; }

        public int? DepartamentoId { get; set; }
        [ForeignKey(nameof(DepartamentoId))]
        public virtual Departamento? Departamento { get; set; }

        public int? AreaId { get; set; }
        [ForeignKey(nameof(AreaId))]
        public virtual Area? Area { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
