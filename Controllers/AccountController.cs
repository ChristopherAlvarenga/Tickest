using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;

namespace Tickest.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private readonly TickestContext _context;
        private readonly RoleManager<Role> _roleManager;

        public AccountController(UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager, TickestContext context,
            RoleManager<Role> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Gerenciador")]
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            //ViewBag.Departamentos = _context.Departamentos.ToList();

            var roles = await _roleManager.Roles.ToListAsync();

            var viewModel = new RegisterViewModel
            {
                OpcoesFuncoes = roles.Select(p => new FuncaoViewModel { Id = p.Id, Nome = p.Name }).ToList(),
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Gerenciador")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var now = DateTime.Now;

                // Copia os dados do RegisterViewModel para o IdentityUser
                var user = new Usuario
                {
                    Nome = model.Nome,
                    UserName = model.Email,
                    Email = model.Email,
                    NormalizedUserName = model.Nome.ToUpper(),
                    NormalizedEmail = model.Email.ToUpper(),
                    EspecialidadeId = model.EspecialidadeId, 
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                };

                // Armazena os dados do usuário na tabela AspNetUsers
                var result = await userManager.CreateAsync(user, model.Senha);

                // Se o usuário foi criado com sucesso, faz o login do usuário usando o serviço
                // SignInManager e redireciona para o método Action Index
                if (result.Succeeded)
                {
                    var funcaoSelecionada = await _roleManager.FindByIdAsync(model.FuncaoId.ToString());

                    await userManager.AddToRoleAsync(user, funcaoSelecionada.Name);
                    return RedirectToAction("Index", "Gerenciador");
                }

                // Se houver erros, inclui no ModelState e exibe pela tag helper summary na validação
                if (result.Errors != null)
                {
                    ModelState.AddModelError(string.Empty, "A senha deve possuir mais de 6 caracteres.");
                    ModelState.AddModelError(string.Empty, "A senha deve ter pelo menos um caractere especial ('@','#', '&', etc).");
                    ModelState.AddModelError(string.Empty, "A senha deve ter pelo menos uma letra minúscula ('a'-'z').");
                    ModelState.AddModelError(string.Empty, "A senha deve ter pelo menos uma letra maiúscula ('A'-'Z').");
                }
            }

            var roles = await _roleManager.Roles.ToListAsync();

            var viewModel = new RegisterViewModel
            {
                OpcoesFuncoes = roles.Select(p => new FuncaoViewModel { Id = p.Id, Nome = p.Name }).ToList()
            };

            return View(viewModel);
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
            var user = await userManager.FindByEmailAsync(loginModel.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Usuário não encontrado Inválido");
                return View(loginModel);
            }

            var result = await signInManager.PasswordSignInAsync(user, loginModel.Senha, false, false);
            if (!result.Succeeded)
                ModelState.AddModelError(string.Empty, "Email ou senha Inválido");

            if (!ModelState.IsValid)
                return View(loginModel);

            if (await userManager.IsInRoleAsync(user, "Gerenciador"))
                return RedirectToAction("Index", "Gerenciador");

            else if (await userManager.IsInRoleAsync(user, "Cliente"))
                return RedirectToAction("Index", "Tickets");
            else
                return RedirectToAction("Index", "Analista");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<object> PodeSelecionarDepartamento(int roleId)
        {
            if (roleId == 0)
                return new { result = false };

            var role = await _roleManager.FindByIdAsync(roleId.ToString());

            var result = role.Name == "Analista" || role.Name == "Gerenciador";

            return new { result };
        }
    }
}
