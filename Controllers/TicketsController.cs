using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;
using Tickest.Enums;

namespace Tickest.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        #region Constructor and Dependencies

        private readonly UserManager<Usuario> _userManager;
        private readonly TickestContext _context;

        public TicketsController(UserManager<Usuario> userManager, TickestContext context)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #endregion

        #region Actions

        public async Task<IActionResult> Index()
        {
            var usuarioAtual = await _userManager.GetUserAsync(User);

            var tickets = await _context.Tickets
                .Include(t => t.Departamento)
                .Include(t => t.Responsavel)
                .Include(t => t.Anexos)
                .Include(t => t.Solicitante)
                .Where(t => t.SolicitanteId == usuarioAtual.Id)
                .Where(t => t.Status != TicketStatusEnum.Concluido && t.Status != TicketStatusEnum.Cancelado)
                .OrderBy(t => t.Id)
                .ToListAsync();

            var viewModel = new TicketViewModel
            {
                Tickets = tickets,
                Usuario = usuarioAtual
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Historic()
        {
            var usuarioAtual = await _userManager.GetUserAsync(User);

            var tickets = await _context.Tickets
                .Include(t => t.Departamento)
                .Include(t => t.Responsavel)
                .Include(t => t.Solicitante)
                .Include(t => t.Anexos)
                .Where(t => t.Responsavel.Email == usuarioAtual.Email || t.SolicitanteId == usuarioAtual.Id)
                .Where(t => t.Status == TicketStatusEnum.Concluido || t.Status == TicketStatusEnum.Cancelado)
                .OrderBy(t => t.Id)
                .ToListAsync();

            var viewModel = new TicketViewModel
            {
                Tickets = tickets,
                Usuario = usuarioAtual
            };

            return View(viewModel);
        }

        public IActionResult Search(string search)
        {
            var usuario = _context.Usuarios
               .FirstOrDefault(u => u.Email == User.Identity.Name);

            var tickets = _context.Tickets
                .Include(t => t.Departamento)
                .Include(t => t.Responsavel)
                .Include(t => t.Anexos)
                .Where(t => t.Responsavel.Email == usuario.Email && (EF.Functions.Like(t.Titulo, "%" + search + "%") || (t.Id.ToString() == search) || (EF.Functions.Like(t.Descricao, "%" + search + "%") || (EF.Functions.Like(t.Departamento.Nome, "%" + search + "%")))))
                .OrderBy(t => t.Id)
                .ToList();

            var viewModel = new TicketViewModel
            {
                Tickets = tickets,
                Usuario = usuario
            };

            ViewBag.titulo = "Pesquisando por " + search;

            return View(viewModel);
        }

        public IActionResult Create()
        {
            var user = _context.Usuarios
                .Include(u => u.Departamento)
                .FirstOrDefault(u => u.Email == User.Identity.Name);

            var departamentos = _context.Departamentos
                .OrderBy(d => d.Nome)
                .ToList();

            var especialidades = _context.Especialidades
                .OrderBy(e => e.Nome)
                .ToList();

            var viewModel = new TicketViewModel
            {
                Departamentos = departamentos,
                Especialidades = especialidades
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket, int departamentoId, List<IFormFile> files)
        {
            ViewBag.Especialidades = _context.Especialidades.Where(s => s.DepartamentoId == departamentoId);

            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Email == User.Identity.Name);

            foreach (var file in files)
            {
                var path = await WriteFileAsync(file);
                var fileName = Path.GetFileName(path);
                var name = "anexos/" + fileName;
                var anexo = new Anexo { Endereco = name };
                ticket.Anexos.Add(anexo);
            }

            ticket.DataCriacao = DateTime.Now;
            ticket.Status = TicketStatusEnum.Aberto;
            ticket.DataStatus = DateTime.Now;
            ticket.SolicitanteId = usuario.Id;
            ticket.DepartamentoId = departamentoId; // Corrigido aqui
            _context.Add(ticket);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MudarStatus(int? id, [FromQuery(Name = "status")] TicketStatusEnum? status) // Corrigido aqui
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == User.Identity.Name);

            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            ticket.SolicitanteId = usuario.Id;
            ticket.Status = status ?? TicketStatusEnum.Aberto; // Corrigido aqui
            ticket.DataStatus = DateTime.Now;

            await _context.SaveChangesAsync();

            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }
            else if (await _userManager.IsInRoleAsync(user, "Gerenciador"))
            {
                return RedirectToAction("Index", "Gerenciador");
            }
            else if (await _userManager.IsInRoleAsync(user, "Responsavel"))
            {
                return RedirectToAction("Index", "Responsaveis");
            }
            else
            {
                return RedirectToAction("Index", "Desenvolvedores");
            }
        }

        #endregion

        #region Helpers

        public static async Task<string> WriteFileAsync(IFormFile file)
        {
            string caminhoCompleto = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\anexos");

            if (!Directory.Exists(caminhoCompleto))
            {
                Directory.CreateDirectory(caminhoCompleto);
            }
            string path = Path.Combine(caminhoCompleto, GetTimestamp(DateTime.Now) + Path.GetExtension(file.FileName));
            string name = Path.GetFileName(path);
            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return path;
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        #endregion
    }
}
