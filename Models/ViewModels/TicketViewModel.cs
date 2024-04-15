using Tickest.Models.Entities;

namespace Tickest.Models.ViewModels
{
    public class TicketViewModel
    {
        public Ticket Ticket { get; set; }
        public Usuario Usuario { get; set; }
        public Departamento Departamento { get; set; }
        public Especialidade Especialidade { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<Usuario> Usuarios { get; set; }
        public ICollection<Departamento> Departamentos { get; set; }
        public ICollection<Especialidade> Especialidades { get; set; }
    }
}
