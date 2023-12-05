using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tickest.Data;
using Tickest.Models.Entities;
using Tickest.Models.ViewModels;

namespace Tickest.Services.Authentication
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TickestContext _context; // Adicionei a palavra-chave readonly

        public AccountService(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            IHttpContextAccessor httpContextAccessor,
            TickestContext context,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _roleManager = roleManager;
        }

        //Obter usuário logado
        public async Task<Usuario> GetCurrentUserAsync()
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (currentUser == null)
                return null;

            return await _context.Usuarios.FirstOrDefaultAsync(p => p.Id == currentUser.Id);
        }

        public async Task<ServiceResult> RegisterAsync(UsuarioRegisterViewModel model)
        {
            if (model == null)
                return new ServiceResult(new List<string> { "Dados inválidos" });

            var serviceResult = new ServiceResult();

            // Validação
            if (string.IsNullOrEmpty(model.Email))
                serviceResult.AddError(nameof(UsuarioRegisterViewModel.Email), "Email não pode ser vazio");

            if (string.IsNullOrEmpty(model.Nome))
                serviceResult.AddError(nameof(UsuarioRegisterViewModel.Nome), "Nome não pode ser vazio");

            if (string.IsNullOrEmpty(model.Senha))
                serviceResult.AddError(nameof(UsuarioRegisterViewModel.Senha), "Senha não pode ser vazia");

            // Confirmação
            if (string.IsNullOrEmpty(model.ConfirmarSenha))
                serviceResult.AddError(nameof(UsuarioRegisterViewModel.ConfirmarSenha), "Confirmação de senha não pode ser vazia");

            if (model.Senha != model.ConfirmarSenha)
                serviceResult.AddError(nameof(UsuarioRegisterViewModel.ConfirmarSenha), "As senhas não coincidem");

            if (!serviceResult.Success)
                return serviceResult;

            // Copia os dados do RegisterViewModel para o IdentityUser
            var user = new Usuario
            {
                UserName = model.Nome,
                Email = model.Email,
                NormalizedUserName = model.Nome.ToUpper(),
                NormalizedEmail = model.Email.ToUpper(),
                Nome = model.Nome,
                //CargoId = model.car,
                DepartamentoId = model.DepartamentoSelecionado
            };

            // Armazena os dados do usuário na tabela AspNetUsers
            var identityResult = await _userManager.CreateAsync(user, model.Senha);

            if (!identityResult.Succeeded)
                serviceResult.AddError(string.Empty, "Erro ao incluir o usuário");

            if (!serviceResult.Success)
                return serviceResult;

            await _userManager.AddToRoleAsync(user, model.UserRole.Nome);

            return serviceResult;
        }

        public async Task<ServiceResult> LoginAsync(LoginViewModel loginModel)
        {
            if (loginModel == null)
                return new ServiceResult(new List<string> { "Dados inválidos" });

            var serviceResult = new ServiceResult();

            // Validação
            if (string.IsNullOrEmpty(loginModel.Email))
                serviceResult.AddError(nameof(LoginViewModel.Email), "Email não pode ser vazio");

            if (string.IsNullOrEmpty(loginModel.Senha))
                serviceResult.AddError(nameof(LoginViewModel.Senha), "Senha não pode ser vazia");

            if (!serviceResult.Success)
                return serviceResult;

            var result = await _signInManager.PasswordSignInAsync(
                    loginModel.Email, loginModel.Senha, loginModel.ManterLogin, false);

            if (!result.Succeeded)
                serviceResult.AddError(nameof(LoginViewModel.Senha), "Credenciais inválidas");

            return serviceResult;
        }

        public async Task<bool> CreateUserWithRoleAsync(string email, string password, string role)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
                return false;

            var user = new Usuario
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                return true;
            }

            return false;
        }
    }
}
