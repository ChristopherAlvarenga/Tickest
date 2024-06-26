﻿using Microsoft.AspNetCore.Authorization;
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
    public class AnalistaController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly TickestContext _context;

        public AnalistaController(UserManager<Usuario> userManager, TickestContext context)
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
                .Include(p => p.Responsavel)
                .Include(p => p.Solicitante)
                .Include(p => p.Anexos)
                .Where(p => p.EspecialidadeId == usuario.EspecialidadeId)
                .Where(p => p.Status != Ticket.Tipo.Concluido)
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
                    Titulo = p.Titulo,
                    Descricao = p.Descricao,
                    DataCriacao = p.DataCriacao,
                    Status = p.Status,
                    DataStatus = p.DataStatus,
                    Prioridade = p.Prioridade,
                    Responsavel = p.Responsavel,
                    Departamento = p.Departamento,
                    SolicitanteId = p.SolicitanteId,
                    Solicitante = p.Solicitante,
                    Anexos = p.Anexos
                }).ToList(),
                Usuario = usuario
            };

            ViewBag.TicketsAberto = _context.Tickets
                .Where(p => p.SolicitanteId == usuario.Id)
                .Where(p => p.Status != Ticket.Tipo.Cancelado && p.Status != Ticket.Tipo.Concluido)
                .Count();

            ViewBag.TicketsRecebidos = _context.Tickets
                .Where(p => p.SolicitanteId == usuario.Id)
                .Where(p => p.Status != Ticket.Tipo.Cancelado)
                .Where(p => p.DataCriacao.Month == DateTime.Now.Month)
                .Count();

            ViewBag.TicketConcluidos = _context.Tickets
                .Where(p => p.SolicitanteId == usuario.Id)
                .Where(p => p.Status == Ticket.Tipo.Concluido)
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