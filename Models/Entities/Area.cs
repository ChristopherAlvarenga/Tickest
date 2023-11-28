using System.ComponentModel.DataAnnotations.Schema;

namespace Tickest.Models.Entities
{
    public class Area
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public int? DepartamentoId { get; set; }
        [ForeignKey(nameof(DepartamentoId))]
        public virtual Departamento? Departamento { get; set; }

        public ICollection<Usuario> Usuarios { get; set; }
    }
}
