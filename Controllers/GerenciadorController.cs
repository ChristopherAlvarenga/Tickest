using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Tickest.Data;
using Tickest.Enums;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;

namespace Tickest.Controllers
{
    [Authorize(Roles = "Gerenciador")]
    public class GerenciadorController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly TickestContext _context;

        public GerenciadorController(UserManager<Usuario> userManager, TickestContext context)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IActionResult Index()
        {
            var usuario = _userManager.FindByEmailAsync(User.Identity.Name).Result;

            var tickets = _context.Tickets
                .Include(t => t.Departamento)
                .Include(t => t.Responsavel)
                .Include(t => t.Solicitante)
                .Include(t => t.Anexos)
                .Where(t => t.EspecialidadeId == usuario.EspecialidadeId &&
                            (t.Status != TicketStatusEnum.Concluido && t.Status != TicketStatusEnum.Cancelado))
                .ToList();

            tickets.RemoveAll(t => t.Status != TicketStatusEnum.Aberto && t.SolicitanteId != usuario.Id);

            var viewModel = new TicketViewModel
            {
                Tickets = tickets,
                Usuario = usuario
            };

            ViewBag.TicketsAberto = _context.Tickets
                .Count(t => t.SolicitanteId == usuario.Id &&
                            (t.Status != TicketStatusEnum.Cancelado && t.Status != TicketStatusEnum.Concluido));

            ViewBag.TicketsRecebidos = _context.Tickets
                .Count(t => t.SolicitanteId == usuario.Id &&
                            (t.Status != TicketStatusEnum.Cancelado && t.DataCriacao.Month == DateTime.Now.Month));

            ViewBag.TicketConcluidos = _context.Tickets
                .Count(t => t.SolicitanteId == usuario.Id &&
                            (t.Status == TicketStatusEnum.Concluido && t.DataStatus.Month == DateTime.Now.Month));

            return View(viewModel);
        }

        public IActionResult AllTickets()
        {
            var tickets = _context.Tickets
                .Include(t => t.Departamento)
                .Include(t => t.Responsavel)
                .Include(t => t.Solicitante)
                .Include(t => t.Anexos)
                .ToList();

            var viewModel = new TicketViewModel
            {
                Tickets = tickets
            };

            return View(viewModel);
        }
    }
}
