using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using Tickest.Areas.Gerenciador.ViewModels;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;
using Tickest.Services.Authentication;
using static Tickest.Models.Entities.Ticket;

namespace Tickest.Controllers
{
    [Area("Gerenciador")]
    [Authorize(Policy = "AdminGerenciadorPolicy")]
    public class GerenciadorController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly TickestContext _context;

        public GerenciadorController(
            IAuthenticationService authenticationService,
            TickestContext context)
        {
            _authenticationService = authenticationService;
            _context = context;
        }

        #region Seed Ticket

        private async Task SeedTicket()
        {
            var tickets = new List<Ticket>()
            {
                new Ticket
                {
                    Titulo = "Problema no Carrinho",
                    Descricao = "Eu como usuário não estou conseguindo acessar meu Carrinho",
                    Comentario = "Recebo erro de Carrinho inválido",
                    DataCriacao = DateTime.Now.AddDays(3),
                    DataLimite = DateTime.Now.AddDays(4),
                    Prioridade = Escolha.Baixa,
                    Status = new List<TicketStatus>
                    {
                        new TicketStatus{ Status = TicketStatusEnum.Aberto, DataAlteracao =  DateTime.Now.AddDays(3)},
                    }
                },

                new Ticket
                {
                    Titulo = "Problema no Login",
                    Descricao = "Eu como usuário não estou conseguindo acessar minha conta",
                    Comentario = "Recebo erro de usuário inválido",
                    DataCriacao = DateTime.Now.AddDays(1),
                    DataLimite = DateTime.Now.AddDays(4),
                    Prioridade = Escolha.Baixa,
                    Status = new List<TicketStatus>
                    {
                        new TicketStatus{ Status = TicketStatusEnum.Aberto, DataAlteracao =  DateTime.Now.AddDays(1)},
                        new TicketStatus{ Status = TicketStatusEnum.Andamento, DataAlteracao =  DateTime.Now.AddMinutes(120)},
                    }
                },
                new Ticket
                {
                    Titulo = "Problema na conta",
                    Descricao = "Eu como usuário não estou conseguindo acessar minha conta pessoal",
                    Comentario = "Recebo erro de conta inválida",
                    DataCriacao = DateTime.Now,
                    DataLimite = DateTime.Now.AddDays(5),
                    Prioridade = Escolha.Media,
                    Status = new List<TicketStatus>
                    {
                        new TicketStatus{ Status = TicketStatusEnum.Aberto, DataAlteracao =  DateTime.Now},
                        new TicketStatus{ Status = TicketStatusEnum.Andamento, DataAlteracao =  DateTime.Now.AddHours(4)},
                    }
                },

                new Ticket
                {
                    Titulo = "Problema no cadastro de usuario",
                    Descricao = "Nâo consigo cadastr um usuario",
                    Comentario = "Recebo erro de entrar em contato com o suporte",
                    DataCriacao = DateTime.Now,
                    DataLimite = DateTime.Now.AddDays(6),
                    Prioridade = Escolha.Alta,
                    Status = new List<TicketStatus>
                    {
                        new TicketStatus{ Status = TicketStatusEnum.Aberto, DataAlteracao =  DateTime.Now},
                        new TicketStatus{ Status = TicketStatusEnum.Andamento, DataAlteracao =  DateTime.Now.AddHours(5)},
                        new TicketStatus{ Status = TicketStatusEnum.Teste, DataAlteracao =  DateTime.Now.AddHours(8)},
                        new TicketStatus{ Status = TicketStatusEnum.Concluido, DataAlteracao =  DateTime.Now.AddDays(3)},
                    }
                },

                new Ticket
                {
                    Titulo = "Problema no computador",
                    Descricao = "Não liga",
                    Comentario = "Nenhuma luz ascende",
                    DataCriacao = DateTime.Now,
                    DataLimite = DateTime.Now.AddDays(7),
                    Prioridade = Escolha.Urgente,
                    Status = new List<TicketStatus>
                    {
                        new TicketStatus{ Status = TicketStatusEnum.Aberto, DataAlteracao =  DateTime.Now},
                        new TicketStatus{ Status = TicketStatusEnum.Andamento, DataAlteracao =  DateTime.Now.AddHours(6)},
                        new TicketStatus{ Status = TicketStatusEnum.Teste, DataAlteracao =  DateTime.Now.AddDays(2)},
                    }
                }
            };

            _context.Tickets.AddRange(tickets);
            await _context.SaveChangesAsync();
        }

        #endregion

        // Recuperar o primeiro dia da semana atual 
        public DateTime GetPrimeiroDiaSemana(DateTime date)
        {
            int daysUntilSunday = (int)date.DayOfWeek;
            return date.AddDays(-daysUntilSunday);
        }

        // Recuperar o último dia da semana atual 
        public DateTime GetUltimoDiaSemana(DateTime date)
        {
            int daysUntilSaturday = 6 - (int)date.DayOfWeek;
            return date.AddDays(daysUntilSaturday);
        }

        public async Task<IActionResult> Index()
        {
            var currentuser = await _authenticationService.GetCurrentUserAsync();

            //TICKECTSTATUS
            //TicketId
            //Status
            //DataAlteracao

            //Ticket id = 1
            //TicketStatus idTicket = 1, status = aberto, data = 10-10-2023
            //TicketStatus idTicket = 1, status = andamento, data = 11-10-2023
            //TicketStatus idTicket = 1, status = finalizado, data = 15-10-2023

            //verificar status dos tickets da semana atual (19/11) e que estão no status desejado

            // HOJE == 19/11/2023

            // PEGAR TICKETS ONDE A DATAALTERACAO DO ULTIMO STATUS ESTEJA ENTRE O PRIMEIRO DIA DA SEMANA E O ULTIMO DIA DA SEMANA  

            var hoje = DateTime.Today;

            var primeiroDiaSemana = GetPrimeiroDiaSemana(hoje);
            var ultimoDiaSemana = GetUltimoDiaSemana(hoje);

            var tickesSemana = _context.Tickets
                // Include -> Join 
                .Include(p => p.Status)
               //Propriedade anônima que foi igualada a 'P' com o nome "Ticket" 
               //Select -> Seleciona Depois 'P' seleciona os "Status" em ordem decrescente por "DataAlteracao" e pega o primeiro que seria o último da lista
               .Select(p => new { Ticket = p, StatusAtual = p.Status.OrderByDescending(x => x.DataAlteracao).FirstOrDefault() })
               //Where -> Filtro
               .Where(p => p.StatusAtual.DataAlteracao.Date >= primeiroDiaSemana && p.StatusAtual.DataAlteracao <= ultimoDiaSemana);

            var ticktesHoje = _context.Tickets
                .Where(p => p.DataCriacao.Date == DateTime.Today.Date);

            var viewModel = new GerenciadorHomeViewModel
            {
                Usuario = currentuser.UserName,
                TicketsTesteSemana = tickesSemana.Count(p => p.StatusAtual.Status == TicketStatusEnum.Teste),
                TicketsAbertoSemana = tickesSemana.Count(p => p.StatusAtual.Status == TicketStatusEnum.Aberto),
                TicketsEmAndamentoSemana = tickesSemana.Count(p => p.StatusAtual.Status == TicketStatusEnum.Andamento),
                TicketsConcluidoSemana = tickesSemana.Count(p => p.StatusAtual.Status == TicketStatusEnum.Concluido),
                TicketsNaoConcluidoSemana = tickesSemana.Count(p => p.StatusAtual.Status != TicketStatusEnum.Concluido),
                UltimosCincoTickets = _context.Tickets.Include(p => p.Status)
                    .OrderByDescending(p => p.DataCriacao)
                    .Take(5)
                    .Select(p => new TicketViewModel
                    {
                        Ticket = p
                    }).ToList(),

                AnaliseHoje = new GerenciadorHomeTicketInfoViewModel
                {
                    Quantidade = ticktesHoje.Count(p => p.Status.OrderByDescending(x => x.DataAlteracao).FirstOrDefault().Status == TicketStatusEnum.Andamento),
                    //setor
                },

                ConcluidoHoje = new GerenciadorHomeTicketInfoViewModel
                {
                    Quantidade = ticktesHoje.Count(p => p.Status.OrderByDescending(x => x.DataAlteracao).FirstOrDefault().Status == TicketStatusEnum.Concluido),
                    //setor
                },


            };
            return View(viewModel);
        }
    }
}
