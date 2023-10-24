using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;

namespace Tickest.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly TickestContext _context;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, TickestContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _context = context;
        }

        [Authorize(Policy = "RequireRole")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [Authorize(Policy = "RequireRole")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            if (ModelState.IsValid)
            {
                // Copia os dados do RegisterViewModel para o IdentityUser
                var user = new IdentityUser
                {
                    UserName = registerModel.Nome,
                    Email = registerModel.Email,
                    NormalizedUserName = registerModel.Nome.ToUpper(),
                    NormalizedEmail = registerModel.Email.ToUpper(),
                };

                // Armazena os dados do usuário na tabela AspNetUsers
                var result = await userManager.CreateAsync(user, registerModel.Senha);

                // Se o usuário foi criado com sucesso, faz o login do usuário usando o serviço
                // SignInManager e redireciona para o método Action Index
                if (result.Succeeded)
                {
                    if(registerModel.Gerenciador == true)
                        await userManager.AddToRoleAsync(user, "Gerenciador");
                    else if (registerModel.Responsavel == true)
                        await userManager.AddToRoleAsync(user, "Responsavel");
                    else
                        await userManager.AddToRoleAsync(user, "Colaborador");

                    var usuario = new Usuario()
                    {
                        Nome = registerModel.Nome,
                        Email = registerModel.Email,
                        CargoId = 1,
                        DepartamentoId = 1
                    };

                    _context.Add(usuario);
                    await _context.SaveChangesAsync();
                }

                // Se houver erros, inclui no ModelState e exibe pela tag helper summary na validação
                foreach (var error in result.Errors) 
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return RedirectToAction("Index", "Gerenciador", new { area = "Gerenciador" });
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(
                    loginModel.Email, loginModel.Senha, loginModel.ManterLogin, false);

                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(loginModel.Email);

                    if (await userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Index", "Admin", new { area = "Admin" });
                    }
                    else if (await userManager.IsInRoleAsync(user, "Gerenciador"))
                    {
                        return RedirectToAction("Index", "Gerenciador", new { area = "Gerenciador" });
                    }
                    else if (await userManager.IsInRoleAsync(user, "Responsavel"))
                    {
                        return RedirectToAction("Index", "Responsavel", new { area = "Responsavel" });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Colaboradores");
                    }

                }

                ModelState.AddModelError(string.Empty, "Login Inválido");
            }
            return View(loginModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
