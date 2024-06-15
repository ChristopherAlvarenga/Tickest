using System.Collections.Generic;
using Tickest.Models.Entities;

namespace Tickest.Models.ViewModels
{
    // ViewModel para exibição e manipulação de tickets.
    public class TicketViewModel
    {
        #region Properties

        // Propriedades do ticket
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Prioridade { get; set; }
        public Departamento Departamento { get; set; }
        public Especialidade Especialidade { get; set; }
        public Usuario Usuario { get; set; }
        public List<Ticket> Tickets { get; set; }

        // Listas de departamentos e especialidades
        public List<Departamento> Departamentos { get; set; }
        public List<Especialidade> Especialidades { get; set; }

        // IDs para associar departamento e especialidade ao ticket
        public int DepartamentoId { get; set; }
        public int EspecialidadeId { get; set; }

        #endregion

        #region Counts

        // Contagens de tickets
        public string TicketsRecebidos { get; set; }
        public string TicketConcluidos { get; set; }
        public string TicketsAberto { get; set; }

        #endregion
    }
}
