﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;

namespace Tickest.Controllers
{
    [Authorize]
    public class DepartamentosController : Controller
    {
        private readonly TickestContext _context;

        public DepartamentosController(TickestContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "RequireRole")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "RequireRole")]
        public IActionResult Create()
        {
            var query = _context.Usuarios.AsQueryable();

            var viewModel = new UsuarioDepartamentoViewModel()
            {
                Usuarios = query.Select(p => new Usuario
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Email = p.Email,
                    CargoId = p.CargoId,
                    DepartamentoId = p.DepartamentoId
                }).ToList()
            };

            return View(viewModel);
        }

        [Authorize(Policy = "RequireRole")]
        [HttpPost]
        public async Task<IActionResult> Create(UsuarioDepartamentoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var departamento = new Departamento()
                {
                    Nome = viewModel.Departamento.Nome,
                    ResponsavelId = viewModel.Departamento.ResponsavelId,
                };

                _context.Add(departamento);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Gerenciador", new { area = "Gerenciador" });
        }
    }
}
