using System.ComponentModel.DataAnnotations;

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

        [Required]
        [StringLength(1000)]
        public string Comentario { get; set; }

        [Required]
        public DateTime Data_Criação { get; set; }

        public DateTime Data_Limite { get; set; }

        [Required]
        [EnumDataType(typeof(Escolha))]
        public Escolha Prioridade { get; set; }
        public enum Escolha
        {
            Baixa = 0, Média = 1, Alta = 2, Urgente = 3
        }

        [Required]
        [EnumDataType(typeof(Tipo))]
        public Tipo Status { get; set; }
        public enum Tipo
        {
            Análise = 0, Andamento = 1, Teste = 2, Concluído = 3, Cancelado = 4
        }

        public ICollection<UsuarioTicket> UsuarioTickets { get; set; }
    }
}
