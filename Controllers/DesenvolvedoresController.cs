using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using System.Linq;
using Tickest.Data;
using Tickest.Models;
using Tickest.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Tickest.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Tickest.Controllers
{

    [Authorize]
    public class DesenvolvedoresController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly TickestContext _context;

        public DesenvolvedoresController(UserManager<IdentityUser> userManager, TickestContext context)
        {
            this.userManager = userManager;
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            var usuario = _context.Usuarios
                .Where(p => p.Email == User.Identity.Name)
                .FirstOrDefault();

            var query = _context.Tickets
                .Include(p => p.Departamento)
                .Include(p => p.Usuario)
                .Include(p => p.Anexos)
                .Where(p => p.AreaId == usuario.AreaId)
                .Where(p => p.UsuarioId != usuario.Id)
                .Where(p => p.Status != Ticket.Tipo.Concluído)
                .Where(p => p.Status != Ticket.Tipo.Cancelado)
                .AsQueryable();

            foreach (var ticket in query)
            {
                if (ticket.Status != Ticket.Tipo.Criado && ticket.DestinatarioId != usuario.Id)
                    query.ToList().RemoveAll(p => p.Id == ticket.Id);
            }

            var viewModel = new TicketViewModel()
            {
                Tickets = query.Select(p => new Ticket
                {
                    Id = p.Id,
                    Título = p.Título,
                    Descrição = p.Descrição,
                    Data_Criação = p.Data_Criação,
                    Status = p.Status,
                    Data_Status = p.Data_Status,
                    Prioridade = p.Prioridade,
                    Usuario = p.Usuario,
                    Departamento = p.Departamento,
                    DepartamentoId = p.DepartamentoId,
                    DestinatarioId = p.DestinatarioId,
                    Anexos = p.Anexos
                }).ToList(),
                Usuario = usuario
            };

            ViewBag.TicketsAberto = _context.Tickets
                .Where(p => p.DestinatarioId == usuario.Id)
                .Where(p => p.Status != Ticket.Tipo.Cancelado && p.Status != Ticket.Tipo.Concluído)
                .Count();

            ViewBag.TicketsRecebidos = _context.Tickets
                .Where(p => p.DestinatarioId == usuario.Id)
                .Where(p => p.Status != Ticket.Tipo.Cancelado)
                .Where(p => p.Data_Criação.Month == DateTime.Now.Month)
                .Count();

            ViewBag.TicketConcluidos = _context.Tickets
                .Where(p => p.DestinatarioId == usuario.Id)
                .Where(p => p.Status == Ticket.Tipo.Concluído)
                .Where(p => p.Data_Status.Month == DateTime.Now.Month)
                .Count();

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}