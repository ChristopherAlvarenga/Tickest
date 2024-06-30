using Microsoft.Identity.Client;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Tickest.Models.Entities
{
    public class Departamento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Nome { get; set; }

        public int GerenciadorId { get; set; }

        public ICollection<Usuario> Usuarios { get; set; }

        public ICollection<Especialidade> Especialidades { get; set; }
    }
}
