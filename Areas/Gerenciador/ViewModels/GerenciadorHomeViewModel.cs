using Tickest.Models.ViewModels;

namespace Tickest.Areas.Gerenciador.ViewModels
{
    public class GerenciadorHomeViewModel
    {
        public GerenciadorHomeViewModel()
        {
            UltimosCincoTickets = new List<TicketViewModel>();
        }

        public string Usuario { get; set; }

        public int TicketsConcluidoSemana { get; set; }
        public int TicketsAbertoSemana { get; set; }
        public int TicketsEmAndamentoSemana { get; set; }
        public int TicketsNaoConcluidoSemana { get; set; }
        public int TicketsTesteSemana { get; set; }

        public ICollection<TicketViewModel> UltimosCincoTickets { get; set; }

        public GerenciadorHomeTicketInfoViewModel AnaliseHoje { get; set; }
        public GerenciadorHomeTicketInfoViewModel AndamentoHoje { get; set; }
        public GerenciadorHomeTicketInfoViewModel TesteoHoje { get; set; }
        public GerenciadorHomeTicketInfoViewModel ConcluidoHoje { get; set; }
    }

    public class GerenciadorHomeTicketInfoViewModel
    {
        public int Quantidade { get; set; }
        public string Setor { get; set; }
    }
}
