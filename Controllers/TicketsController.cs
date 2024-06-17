using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly UserManager<Usuario> _userManager;
        private readonly TickestContext _context;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(UserManager<Usuario> userManager, TickestContext context, ILogger<TicketsController> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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
                .Where(t => t.Responsavel.Email == usuario.Email
                    && (EF.Functions.Like(t.Titulo, $"%{search}%")
                    || t.Id.ToString() == search
                    || EF.Functions.Like(t.Descricao, $"%{search}%")
                    || EF.Functions.Like(t.Departamento.Nome, $"%{search}%")))
                .OrderBy(t => t.Id)
                .ToList();

            var viewModel = new TicketViewModel
            {
                Tickets = tickets,
                Usuario = usuario
            };

            ViewBag.Titulo = $"Pesquisando por {search}";

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

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    var path = await WriteFileAsync(file);
                    var fileName = Path.GetFileName(path);
                    var name = "anexos/" + fileName;
                    var anexo = new Anexo { Endereco = name };
                    ticket.Anexos.Add(anexo);
                }
            }

            ticket.DataCriacao = DateTime.Now;
            ticket.Status = TicketStatusEnum.Aberto;
            ticket.DataStatus = DateTime.Now;
            ticket.SolicitanteId = usuario.Id;
            ticket.DepartamentoId = departamentoId;
            _context.Add(ticket);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var ticket = _context.Tickets
                .Include(t => t.Departamento)
                .FirstOrDefault(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            var viewModel = new TicketViewModel
            {
                Ticket = ticket,
                Departamentos = _context.Departamentos.OrderBy(d => d.Nome).ToList(),
                Especialidades = _context.Especialidades.OrderBy(e => e.Nome).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EditPolicy")]
        public async Task<IActionResult> Editar(int id, Ticket ticket, int departamentoId, List<IFormFile> files)
        {
            if (id != ticket.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                var viewModel = new TicketViewModel
                {
                    Ticket = ticket,
                    Departamentos = _context.Departamentos.OrderBy(d => d.Nome).ToList(),
                    Especialidades = _context.Especialidades.OrderBy(e => e.Nome).ToList()
                };

                return View(viewModel);
            }

            var usuario = await _userManager.GetUserAsync(User);

            var ticketExistente = await _context.Tickets
                .Include(t => t.Anexos)
                .FirstOrDefaultAsync(t => t.Id == ticket.Id);

            if (ticketExistente == null)
            {
                return NotFound();
            }

            // Verifique se o usuário tem permissão para editar o ticket
            if (ticketExistente.ResponsavelId != usuario.Id && !User.IsInRole("Gerenciador"))
            {
                return Forbid();
            }

            // Atualizar propriedades do ticket
            ticketExistente.Titulo = ticket.Titulo;
            ticketExistente.Descricao = ticket.Descricao;
            ticketExistente.Prioridade = ticket.Prioridade;
            ticketExistente.EspecialidadeId = ticket.EspecialidadeId;
            ticketExistente.ResponsavelId = ticket.ResponsavelId;

            // Adicionar anexos
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    var path = await WriteFileAsync(file);
                    var fileName = Path.GetFileName(path);
                    var name = "anexos/" + fileName;
                    var anexo = new Anexo { Endereco = name };
                    ticketExistente.Anexos.Add(anexo);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(ticket.Id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Erro de concorrência ao atualizar o ticket.");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao atualizar o ticket: {ex.Message}");
                throw; // Ou redirecionamento para uma página de erro personalizada
            }

            return RedirectToAction(nameof(Details), new { id = ticket.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Departamento)
                .Include(t => t.Responsavel)
                .Include(t => t.Anexos)
                .Include(t => t.Solicitante)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            var viewModel = new TicketViewModel
            {
                Ticket = ticket,
                Usuario = await _userManager.GetUserAsync(User)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> MudarStatus(int? id, [FromQuery(Name = "status")] TicketStatusEnum? status)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            ticket.Status = status ?? TicketStatusEnum.Aberto;
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

        #region Helpers

        private async Task<string> WriteFileAsync(IFormFile file)
        {
            string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/anexos");
            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }

            string filePath = Path.Combine(uploads, GetTimestamp(DateTime.Now) + Path.GetExtension(file.FileName));
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return filePath;
        }

        private static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }

        #endregion
    }
}
