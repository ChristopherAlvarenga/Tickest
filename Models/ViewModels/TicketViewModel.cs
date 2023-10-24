using Tickest.Models.Entities;

namespace Tickest.Models.ViewModels
{
    public class TicketViewModel
    {
        public Ticket Ticket { get; set; }
        public Departamento Departamento { get; set; }
        public Area Area { get; set; }
        public ICollection<Departamento> Departamentos { get; set; }
        public ICollection<Area> Areas { get; set; }
    }
}
