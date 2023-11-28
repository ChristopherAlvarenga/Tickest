using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Tickest.Models.Entities
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Título { get; set; }

        [Required]
        [StringLength(1000)]
        public string Descrição { get; set; }

        [AllowNull]
        [StringLength(1000)]
        public string Comentario { get; set; }

        [Required]
        public DateTime Data_Criação { get; set; }

        [Required]
        [EnumDataType(typeof(Escolha))]
        public Escolha Prioridade { get; set; }
        public enum Escolha
        {
            Baixa = 1, Média = 2, Alta = 3, Urgente = 4
        }

        [Required]
        [EnumDataType(typeof(Tipo))]
        public Tipo Status { get; set; }
        public enum Tipo
        {
            Criado = 1, Andamento = 2, Teste = 3, Concluído = 4, Cancelado = 5
        }

        public int? DepartamentoId { get; set; }
        [ForeignKey(nameof(DepartamentoId))]
        public virtual Departamento? Departamento { get; set; }

        public int? AreaId { get; set; }
        [ForeignKey(nameof(AreaId))]
        public virtual Area? Area { get; set; }

        public int? DestinatarioId { get; set; }
        public int? UsuarioId { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        public virtual Usuario? Usuario { get; set; }

        public ICollection<Anexo> Anexos { get; set; }
    }
}
