using System.ComponentModel.DataAnnotations;

namespace Tickest.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Título { get; set; }

        [Required]
        [StringLength(500)]
        public required string Descrição { get; set; }

        [Required]
        [StringLength(500)]
        public required string Comentario { get; set; }

        [Required]
        public required DateTime Data_Criação { get; set; }

        public required DateTime Data_Limite { get; set; }

        [Required]
        [EnumDataType(typeof(Escolha))]
        public required Escolha Prioridade { get; set; }
        public enum Escolha
        {
            Baixa = 0, Média = 1, Alta = 2, Urgente = 3
        }

        [Required]
        [EnumDataType(typeof(Tipo))]
        public required Tipo Status { get; set; }
        public enum Tipo
        {
            Análise = 0, Andamento = 1, Teste = 2, Concluído = 3, Cancelado = 4
        }

        public required ICollection<UsuarioTicket> UsuarioTickets { get; set; }
    }
}
