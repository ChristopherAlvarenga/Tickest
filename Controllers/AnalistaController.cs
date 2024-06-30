using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tickest.Data;
using Tickest.Enums;
using Tickest.Models;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;
using Tickest.Models.ViewModels.Shared;

namespace Tickest.Controllers
{
    [Authorize]
    public class AnalistaController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly TickestContext _context;
        public AnalistaController(UserManager<Usuario> userManager, TickestContext context)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IActionResult Index()
        {
            var user = _userManager.GetUserAsync(User).Result;

            var query = _context.Tickets
                .Include(p => p.Departamento)
                .Include(p => p.Responsavel)
                .Include(p => p.Solicitante)
                .Include(p => p.Anexos)
                .Where(p => p.EspecialidadeId == user.EspecialidadeId)
                .Where(p => p.Status != TicketStatusEnum.Concluido && p.Status != TicketStatusEnum.Cancelado)
                .AsQueryable();

            // Remover tickets que não foram criados pelo usuário atual
            query = query.Where(p => p.Status == TicketStatusEnum.Aberto || p.SolicitanteId == user.Id);

            var viewModel = new TicketViewModel()
            {
                Tickets = query.ToList(),
                Usuario = user
            };

            ViewBag.TicketsAberto = _context.Tickets
                .Where(p => p.SolicitanteId == user.Id)
                .Where(p => p.Status != TicketStatusEnum.Cancelado && p.Status != TicketStatusEnum.Concluido)
                .Count();

            ViewBag.TicketsRecebidos = _context.Tickets
                .Where(p => p.SolicitanteId == user.Id)
                .Where(p => p.Status != TicketStatusEnum.Cancelado)
                .Where(p => p.DataCriacao.Month == DateTime.Now.Month)
                .Count();

            ViewBag.TicketConcluidos = _context.Tickets
                .Where(p => p.SolicitanteId == user.Id)
                .Where(p => p.Status == TicketStatusEnum.Concluido)
                .Where(p => p.DataStatus.Month == DateTime.Now.Month)
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
