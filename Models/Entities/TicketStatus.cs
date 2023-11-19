namespace Tickest.Models.Entities
{
    public class TicketStatus
    {
        public TicketStatus()
        {
                
        }

        public TicketStatus(TicketStatusEnum status)
        {
            Status = status;
            DataAlteracao = DateTime.Now;
        }

        public int Id { get; set; }
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
        public TicketStatusEnum Status { get; set; }
        public DateTime DataAlteracao { get; set; }
    }

    public enum TicketStatusEnum
    {
        Aberto = 1,
        Andamento = 2,
        Concluido = 3,
        Teste = 4,
        Cancelado = -1,
    }
}