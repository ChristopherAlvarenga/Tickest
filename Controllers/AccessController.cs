using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Tickest.Models;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;

namespace Tickest.Controllers
{
    public class AccessController : Controller
    {
        private readonly Contexto _context;

        public AccessController(Contexto context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(VMUsuario vmlogin)
        {
            Usuario usuario = _context.Usuarios
                .FirstOrDefault(p => p.Email == vmlogin.Email &&
                p.Senha == vmlogin.Senha);

            //if (vmlogin.Email == ))
            //{

            //}

            return View();
        }
    }
}
