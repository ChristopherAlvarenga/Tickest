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
    [Authorize(Roles = "Gerenciador, Responsavel")]
    public class ResponsaveisController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly TickestContext _context;

        public ResponsaveisController(UserManager<Usuario> userManager, TickestContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            var usuario = _userManager.FindByEmailAsync(User.Identity.Name).Result;

            var tickets = _context.Tickets
                .Include(t => t.Departamento)
                .Include(t => t.Responsavel)
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

            PopulateTicketCounts(viewModel, usuario);

            return View(viewModel);
        }

        public IActionResult Departamento()
        {
            var usuario = _userManager.FindByEmailAsync(User.Identity.Name).Result;

            var tickets = _context.Tickets
                .Include(t => t.Departamento)
                .Include(t => t.Responsavel)
                .Include(t => t.Anexos)
                .Where(t => t.DepartamentoId == usuario.DepartamentoId)
                .ToList();

            var viewModel = new TicketViewModel
            {
                Tickets = tickets,
                Usuario = usuario
            };

            return View(viewModel);
        }

        private void PopulateTicketCounts(TicketViewModel viewModel, Usuario usuario)
        {
            viewModel.TicketsRecebidos = _context.Tickets
                .Count(t => t.SolicitanteId == usuario.Id &&
                            (t.Status != TicketStatusEnum.Cancelado && t.DataCriacao.Month == DateTime.Now.Month)).ToString();

            viewModel.TicketConcluidos = _context.Tickets
                .Count(t => t.SolicitanteId == usuario.Id &&
                            (t.Status == TicketStatusEnum.Concluido && t.DataStatus.Month == DateTime.Now.Month)).ToString();

            viewModel.TicketsAbertos = _context.Tickets
                .Count(t => t.SolicitanteId == usuario.Id && t.Status == TicketStatusEnum.Aberto).ToString();
        }

    }

}
