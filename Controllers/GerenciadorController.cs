﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;

namespace Tickest.Controllers
{
    [Authorize(Roles = "Gerenciador")]
    public class GerenciadorController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly TickestContext _context;

        public GerenciadorController(UserManager<Usuario> userManager, TickestContext context)
        {
            this.userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            var usuario = _context.Usuarios
                .Where(p => p.Email == User.Identity.Name)
                .FirstOrDefault();

            var query = _context.Tickets
                .Include(p => p.Departamento)
                .Include(p => p.Responsavel)
                .Include(p => p.Anexos)
                .Where(p => p.EspecialidadeId == usuario.EspecialidadeId)
                .Where(p => p.Status != Ticket.Tipo.Concluído)
                .Where(p => p.Status != Ticket.Tipo.Cancelado)
                .AsQueryable();

            foreach (var ticket in query)
            {
                if (ticket.Status != Ticket.Tipo.Criado && ticket.SolicitanteId != usuario.Id)
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
                    Responsavel = p.Responsavel,
                    Departamento = p.Departamento,
                    SolicitanteId = p.SolicitanteId,
                    Anexos = p.Anexos
                }).ToList(),
                Usuario = usuario
            };

            ViewBag.TicketsAberto = _context.Tickets
                .Where(p => p.SolicitanteId == usuario.Id)
                .Where(p => p.Status != Ticket.Tipo.Cancelado && p.Status != Ticket.Tipo.Concluído)
                .Count();

            ViewBag.TicketsRecebidos = _context.Tickets
                .Where(p => p.SolicitanteId == usuario.Id)
                .Where(p => p.Status != Ticket.Tipo.Cancelado)
                .Where(p => p.Data_Criação.Month == DateTime.Now.Month)
                .Count();

            ViewBag.TicketConcluidos = _context.Tickets
                .Where(p => p.SolicitanteId == usuario.Id)
                .Where(p => p.Status == Ticket.Tipo.Concluído)
                .Where(p => p.Data_Status.Month == DateTime.Now.Month)
                .Count();

            return View(viewModel);
        }

        public IActionResult AllTickets()
        {
            var query = _context.Tickets
                .Include(p => p.Departamento)
                .Include(p => p.Responsavel)
                .Include(p => p.Anexos)
                .AsQueryable();

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
                    Responsavel = p.Responsavel,
                    Departamento = p.Departamento,
                    SolicitanteId = p.SolicitanteId,
                    Anexos = p.Anexos
                }).ToList(),
            };

            return View(viewModel);
        }
    }
}
