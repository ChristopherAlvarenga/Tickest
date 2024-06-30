using System.Collections.Generic;
using Tickest.Models.Entities;

namespace Tickest.Models.ViewModels
{
    // ViewModel para exibição e manipulação de tickets.
    public class TicketViewModel
    {
        #region Propriedades do Ticket

        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Prioridade { get; set; }
        public Departamento Departamento { get; set; }
        public Especialidade Especialidade { get; set; }
        public Usuario Usuario { get; set; }

        #endregion

        public Ticket Ticket { get; set; }  // Propriedade para um único Ticket

        #region Listas e Contagens

        // Listas para seleção
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<Especialidade> Especialidades { get; set; }
        public ICollection<Departamento> Departamentos { get; set; }
        public ICollection<Usuario> Usuarios { get; set; }

        // Contagens de tickets
        public string TicketsRecebidos { get; set; }
        public string TicketConcluidos { get; set; }
        public string TicketsAbertos { get; set; }

        #endregion
    }
}
