using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tickest.Models.Entities
{
    public class Ticket
    {
        public Ticket()
        {
            Anexos = new List<Anexo>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }

        [Required]
        [StringLength(1000)]
        public string Descricao { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; }

        [Required]
        public DateTime DataStatus { get; set; }

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
            Criado = 1, Andamento = 2, Teste = 3, Concluido = 4, Cancelado = 5
        }

        public int? DepartamentoId { get; set; }
        [ForeignKey(nameof(DepartamentoId))]
        public virtual Departamento? Departamento { get; set; }

        public int? EspecialidadeId { get; set; }
        [ForeignKey(nameof(EspecialidadeId))]
        public virtual Especialidade? Especialidade { get; set; }

        public int SolicitanteId { get; set; }
        [ForeignKey(nameof(SolicitanteId))]
        public Usuario Solicitante { get; set; }

        public int? ResponsavelId { get; set; }
        [ForeignKey(nameof(ResponsavelId))]
        public virtual Usuario? Responsavel { get; set; }

        public List<Anexo> Anexos { get; set; }
    }
}
