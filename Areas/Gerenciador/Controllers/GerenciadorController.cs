using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tickest.Areas.Gerenciador.ViewModels;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;
using Tickest.Services.Authentication;

namespace Tickest.Controllers
{
    [Area("Gerenciador")]
    [Authorize(Policy = "AdminGerenciadorPolicy")]
    public class GerenciadorController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly TickestContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public GerenciadorController(
            IAccountService accountService,
            TickestContext context,
            RoleManager<IdentityRole> roleManager)
        {
            _accountService = accountService;
            _context = context;
            _roleManager = roleManager;
        }

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
            var currentuser = await _accountService.GetCurrentUserAsync();

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

        [HttpGet]
        public async Task<IActionResult> CadastarFuncionario()
        {
            //Áreas e Departamentos 
            var viewModel = new UsuarioRegisterViewModel();
            viewModel.Areas = await _context.Areas.Select(p => new AreaViewModel { Id = p.Id, Nome = p.Nome }).ToListAsync();
            viewModel.Departamentos = await _context.Departamentos.Select(p => new DepartamentoViewModel { Id = p.Id, Nome = p.Nome }).ToListAsync();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CadastarFuncionario(UsuarioRegisterViewModel registerModel)
        {
            if (!ModelState.IsValid)
                return View(registerModel);

            IdentityRole role = await _roleManager.FindByNameAsync("Colaborador");

            registerModel.UserRole = new(role.Id, role.Name);

            var registerResult = await _accountService.RegisterAsync(registerModel);

            if (!registerResult.Success)
            {
                foreach (var keyError in registerResult.Errors)
                    ModelState.AddModelError(keyError.Key, keyError.Error);

                return View(registerModel);
            }

            return View(new UsuarioRegisterViewModel());
        }
    }
}
