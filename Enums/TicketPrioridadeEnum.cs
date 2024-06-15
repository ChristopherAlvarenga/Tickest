using System.ComponentModel.DataAnnotations;

namespace Tickest.Enums
{
    /// <summary>
    /// Enumeração que representa a prioridade de um ticket.
    /// </summary>
    public enum TicketPrioridadeEnum
    {
        [Display(Name = "Baixa")]
        Baixa = 1,

        [Display(Name = "Média")]
        Média = 2,

        [Display(Name = "Alta")]
        Alta = 3,

        [Display(Name = "Urgente")]
        Urgente = 4
    }
}
