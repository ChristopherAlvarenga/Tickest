using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Enums;
using Tickest.Helpers;
using Microsoft.EntityFrameworkCore;
using Tickest.Models.ViewModels;


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

        public async Task<IActionResult> Search(string search)
        {
            var usuario = await _userManager.GetUserAsync(User);

            var tickets = await _context.Tickets
                .Include(t => t.Departamento)
                .Include(t => t.Responsavel)
                .Include(t => t.Anexos)
                .Where(t => t.Responsavel.Email == usuario.Email
                    && (EF.Functions.Like(t.Titulo, $"%{search}%")
                    || t.Id.ToString() == search
                    || EF.Functions.Like(t.Descricao, $"%{search}%")
                    || EF.Functions.Like(t.Departamento.Nome, $"%{search}%")))
                .OrderBy(t => t.Id)
                .ToListAsync();

            var viewModel = new TicketViewModel
            {
                Tickets = tickets,
                Usuario = usuario
            };

            ViewBag.Titulo = $"Pesquisando por {search}";

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);

            var departamentos = await _context.Departamentos
                .OrderBy(d => d.Nome)
                .ToListAsync();

            var especialidades = await _context.Especialidades
                .OrderBy(e => e.Nome)
                .ToListAsync();

            var viewModel = new TicketViewModel
            {
                Departamentos = departamentos,
                Especialidades = especialidades
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketViewModel viewModel, List<IFormFile> files)
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            if (ModelState.IsValid)
            {
                var ticket = viewModel.Ticket;
                var departamentoId = viewModel.Ticket.DepartamentoId;

                // Verifica se o departamentoId existe na base de dados
                var departamentoExistente = await _context.Departamentos.FindAsync(departamentoId);
                if (departamentoExistente == null)
                {
                    ModelState.AddModelError(nameof(Ticket.DepartamentoId), "Departamento não encontrado.");
                    // Recarrega as listas de departamentos e especialidades para exibir no formulário
                    viewModel.Departamentos = await _context.Departamentos.OrderBy(d => d.Nome).ToListAsync();
                    viewModel.Especialidades = await _context.Especialidades.OrderBy(e => e.Nome).ToListAsync();
                    return View(viewModel);
                }

                // Processa os anexos
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

                // Configuração dos campos do ticket
                ticket.DataCriacao = DateTime.Now;
                ticket.Status = TicketStatusEnum.Aberto;
                ticket.DataStatus = DateTime.Now;
                ticket.SolicitanteId = usuario.Id;

                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Se o modelo não for válido, recarrega as listas de departamentos e especialidades para exibir no formulário
            viewModel.Departamentos = await _context.Departamentos.OrderBy(d => d.Nome).ToListAsync();
            viewModel.Especialidades = await _context.Especialidades.OrderBy(e => e.Nome).ToListAsync();
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return BadRequest("O ID do ticket não pode ser nulo.");
            }

            var ticket = await _context.Tickets
                .Include(t => t.Departamento)
                .Include(t => t.Solicitante)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            var viewModel = new TicketViewModel
            {
                Ticket = ticket,
                Departamentos = await _context.Departamentos.OrderBy(d => d.Nome).ToListAsync(),
                Especialidades = await _context.Especialidades.OrderBy(e => e.Nome).ToListAsync()
            };

            return View("~/Views/Gerenciador/AllTickets.cshtml", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EditPolicy")]
        //public async Task<IActionResult> Editar(int id, [Bind("Id, Titulo, Descricao, Prioridade, EspecialidadeId, ResponsavelId, Status")] Ticket ticket, int departamentoId, List<IFormFile> files)
        public async Task<IActionResult> Editar(Ticket ticket, List<IFormFile> files)
        {
            var usuario = await _userManager.GetUserAsync(User);

            var ticketExistente = await _context.Tickets
                .Include(t => t.Anexos)
                .Include(t => t.Solicitante)
                .FirstOrDefaultAsync(t => t.Id == ticket.Id);

            if (ticketExistente == null)
                return NotFound();

            // Verifique se o usuário tem permissão para editar o ticket
            if (ticketExistente.ResponsavelId != usuario.Id && !User.IsInRole("Gerenciador"))
                return Forbid();

            // Atualize apenas as propriedades necessárias do ticket existente
            ticketExistente.Titulo = ticket.Titulo;
            ticketExistente.Status = ticket.Status;

            // Salve as alterações no banco de dados
            _context.Update(ticketExistente);
            await _context.SaveChangesAsync();

            // Redirecione para a página desejada após salvar as alterações
            return RedirectToAction("AllTickets", "Gerenciador");
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

        [Authorize(Roles = "Gerenciador, Analista")]
        public async Task<IActionResult> MudarStatus(int? id, [FromQuery(Name = "status")] TicketStatusEnum? status)
        {
            if (id == null)
            {
                return BadRequest("O ID do ticket não pode ser nulo.");
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            // Verifica se o usuário atual está autorizado a mudar o status
            if (!(await _userManager.IsInRoleAsync(usuario, "Gerenciador") || await _userManager.IsInRoleAsync(usuario, "Analista")))
            {
                return Forbid();
            }

            // Verifica se o status fornecido é válido
            if (status == null || !Enum.IsDefined(typeof(TicketStatusEnum), status))
            {
                return BadRequest("Status de ticket inválido.");
            }

            // Define o status e a data do status do ticket
            ticket.Status = status.Value;
            ticket.DataStatus = DateTime.Now;

            try
            {
                // Salva as alterações no banco de dados
                _context.Update(ticket);
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
                _logger.LogError($"Erro ao atualizar o status do ticket: {ex.Message}");
                throw;
            }

            // Redireciona conforme o papel do usuário
            if (await _userManager.IsInRoleAsync(usuario, "Admin"))
            {
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }
            else if (await _userManager.IsInRoleAsync(usuario, "Gerenciador"))
            {
                return RedirectToAction("Index", "Gerenciador");
            }
            else if (await _userManager.IsInRoleAsync(usuario, "Responsavel"))
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
