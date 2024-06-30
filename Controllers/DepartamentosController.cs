﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;

namespace Tickest.Controllers
{
    [Authorize]
    public class DepartamentosController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly TickestContext _context;

        public DepartamentosController(UserManager<Usuario> userManager, TickestContext context)
        {
            this.userManager = userManager;
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Gerenciador")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var usersGerenciador = await userManager.GetUsersInRoleAsync("Gerenciador");
            var usersAnalista = await userManager.GetUsersInRoleAsync("Analista");

            var viewModel = new DepartamentoEditViewModel()
            {
                Nome = string.Empty,
                GerenciadorSelecionado = 0,
                GerenciadoresDisponiveis = usersGerenciador.Where(p => p.DepartamentoId == null).Select(p => new GerenciadorViewModel { Id = p.Id, Email = p.Email }).ToList(),
                AnalistaSelecionado = 0,
                AnalistasDisponiveis = usersAnalista.Select(e => new AnalistaViewModel { Id = e.Id, Email = e.Email }).ToList()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Gerenciador")]
        [HttpPost]
        public async Task<IActionResult> Create(DepartamentoEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var departamento = new Departamento();
            departamento.Nome = viewModel.Nome;
            departamento.GerenciadorId = viewModel.GerenciadorSelecionado.Value;

            _context.Add(departamento);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

        [Authorize(Roles = "Gerenciador")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usersGerenciador = await userManager.GetUsersInRoleAsync("Gerenciador");
            var usersAnalistas = await userManager.GetUsersInRoleAsync("Analista");

            var departamento = await _context.Set<Departamento>()
                 .Select(departamento => new DepartamentoEditViewModel
                 {
                     Id = departamento.Id,
                     Nome = departamento.Nome,
                     GerenciadorSelecionado = departamento.GerenciadorId,
                     GerenciadoresDisponiveis = usersGerenciador.Select(responsavel => new GerenciadorViewModel
                     {
                         Id = responsavel.Id,
                         Nome = responsavel.Email
                     }).ToList(),
                     AnalistaSelecionado = 0,
                     AnalistasDisponiveis = usersAnalistas.Select(p => new AnalistaViewModel { Id = p.Id, Email = p.Email }).ToList()
                 })
                 .FirstOrDefaultAsync(p => p.Id == id);

            if (departamento == null)
            {
                return NotFound();
            }

            return View(departamento);
        }

        [Authorize(Roles = "Gerenciador")]
        [HttpPost]
        public async Task<IActionResult> Edit(DepartamentoEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var departamento = await _context.Set<Departamento>().FirstAsync(p => p.Id == viewModel.Id);
            departamento.Nome = viewModel.Nome;
            departamento.GerenciadorId = viewModel.GerenciadorSelecionado.Value;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

        [Authorize(Roles = "Gerenciador")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var departamento = await _context.Departamentos.FirstOrDefaultAsync(p => p.Id == id);

            var ticketsDepartamento = _context.Tickets.Where(p => p.DepartamentoId == id);
            var especialidadeDepartamento = _context.Especialidades.Where(p => p.DepartamentoId == id);
            var usuarioDepartamento = _context.Usuarios.Where(p => p.DepartamentoId == id);

            foreach (var item in ticketsDepartamento)
                item.DepartamentoId = null;

            foreach (var item in especialidadeDepartamento)
                item.DepartamentoId = null;

            foreach (var item in usuarioDepartamento)
                item.DepartamentoId = null;

            _context.Remove(departamento);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var departamentos = await _context.Set<Departamento>()
               .Select(departamento => new DepartamentoListViewModel
               {
                   Id = departamento.Id,
                   Nome = departamento.Nome,
                   Gerenciador = _context.Set<Usuario>().First(p => p.Id == departamento.GerenciadorId).Nome
               }).ToListAsync();

            return View(departamentos);
        }

        [HttpGet]
        public async Task<IActionResult> ObterDepartamentos()
        {
            List<DepartamentoListViewModel> departamentos = await _context.Set<Departamento>()
                .Select(departamento => new DepartamentoListViewModel
                {
                    Id = departamento.Id,
                    Nome = departamento.Nome,
                    Gerenciador = _context.Set<Usuario>().First(p => p.Id == departamento.GerenciadorId).Nome
                }).ToListAsync();

            return Json(departamentos);
        }
    }
}
