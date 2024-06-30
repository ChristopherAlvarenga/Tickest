using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tickest.Data;
using Tickest.Models.ViewModels;
using System.Linq;
using Tickest.Enums;

namespace MeuProjeto.Filtros
{
    public class PreencherContagensTicketsFiltro : IActionFilter
    {
        private readonly TickestContext _context;

        public PreencherContagensTicketsFiltro(TickestContext context)
        {
            _context = context;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var usuario = context.HttpContext.User.Identity.Name;

            if (!string.IsNullOrEmpty(usuario))
            {
                var usuarioId = _context.Usuarios.FirstOrDefault(u => u.Email == usuario)?.Id;

                if (usuarioId != null)
                {
                    var viewModel = new TicketViewModel();

                    // Contagem de tickets recebidos no mês atual
                    viewModel.TicketsRecebidos = _context.Tickets
                        .Count(t => t.SolicitanteId == usuarioId &&
                                    t.Status == TicketStatusEnum.Recebido &&
                                    t.DataCriacao.Month == DateTime.Now.Month).ToString();

                    // Contagem de tickets concluídos no mês atual
                    viewModel.TicketConcluidos = _context.Tickets
                        .Count(t => t.SolicitanteId == usuarioId &&
                                    t.Status == TicketStatusEnum.Concluido &&
                                    t.DataStatus.Month == DateTime.Now.Month).ToString();

                    // Contagem de tickets abertos
                    viewModel.TicketsAbertos = _context.Tickets
                        .Count(t => t.SolicitanteId == usuarioId &&
                                    t.Status == TicketStatusEnum.Aberto).ToString();

                    // Armazenando o viewModel na propriedade Items do contexto HTTP para ser acessível na view
                    context.HttpContext.Items["ContagensTicketsViewModel"] = viewModel;
                }
            }
        }


        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Não é necessário implementar nada aqui neste filtro.
        }
    }
}
