using System.ComponentModel.DataAnnotations;

namespace Tickest.Enums
{
    /// <summary>
    /// Enumeração que representa o status de um ticket.
    /// </summary>
    public enum TicketStatusEnum
    {
        [Display(Name = "Aberto")]
        Aberto = 1,

        [Display(Name = "Recebido")]
        Recebido = 2,

        [Display(Name = "Em Andamento")]
        EmAndamento = 3,

        [Display(Name = "Em Teste")]
        EmTeste = 4,

        [Display(Name = "Concluído")]
        Concluido = 5,

        [Display(Name = "Cancelado")]
        Cancelado = 6
    }
}
