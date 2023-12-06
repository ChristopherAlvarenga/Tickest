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
        private readonly UserManager<IdentityUser> userManager;
        private readonly TickestContext _context;

        public GerenciadorController(UserManager<IdentityUser> userManager, TickestContext context)
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
                .Include(p => p.Usuario)
                .Include(p => p.Anexos)
                .Where(p => p.AreaId == usuario.AreaId)
                .Where(p => p.Status != Ticket.Tipo.Concluído)
                .Where(p => p.Status != Ticket.Tipo.Cancelado)
                .AsQueryable();

            var viewModel = new TicketViewModel()
            {
                Tickets = query.Select(p => new Ticket
                {
                    Id = p.Id,
                    Título = p.Título,
                    Descrição = p.Descrição,
                    Data_Criação = p.Data_Criação,
                    Comentario = p.Comentario,
                    Status = p.Status,
                    Prioridade = p.Prioridade,
                    Usuario = p.Usuario,
                    Departamento = p.Departamento,
                    Anexos = p.Anexos
                }).ToList(),
                Usuario = usuario
            };

            return View(viewModel);
        }
    }
}