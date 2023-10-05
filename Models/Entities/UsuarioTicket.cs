using System.ComponentModel;

namespace Tickest.Models.Entities
{
    public class UsuarioTicket
    {
        public int Id { get; set; }

        [DisplayName("Usuario")]
        public int UsuarioId { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        public virtual Usuario Usuario { get; set; }

        public virtual Ticket Ticket { get; set; }
    }
}
