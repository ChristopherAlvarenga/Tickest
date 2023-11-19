using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Tickest.Models.Entities
{
    public class Ticket
    {
        public Ticket()
        {
            Status = new List<TicketStatus>();
            UsuarioTickets = new List<UsuarioTicket>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }

        [Required]
        [StringLength(1000)]
        public string Descricao { get; set; }

        [AllowNull]
        [StringLength(1000)]
        public string Comentario { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; }

        [Required]
        public DateTime DataLimite { get; set; }

        [Required]
        [EnumDataType(typeof(Escolha))]
        public Escolha Prioridade { get; set; }
        public enum Escolha
        {
            Baixa = 1, Media = 2, Alta = 3, Urgente = 4
        }

        //[Required]
        //[EnumDataType(typeof(Tipo))]
        //public Tipo Status { get; set; }
        //public enum Tipo
        //{
        //    Aberto = 1, Andamento = 2, Teste = 3, Concluido = 4, Cancelado = 5
        //}

        public ICollection<TicketStatus> Status { get; set; }

        public ICollection<UsuarioTicket> UsuarioTickets { get; set; }


        public TicketStatus GetStatusAtual()
        {
            /*
            Lista ordenada com a classe "OrderByDescending" -> pela data.
            TICKET ARRUMAR COMPUTADOR - ID = 1
            CONLUIDO  - 21/11/23
            ANDAMENTO - 20/11/23
            ABERTO    - 19/11/23
            */

            return Status.OrderByDescending(p => p.DataAlteracao).FirstOrDefault();
        }
    }
}
