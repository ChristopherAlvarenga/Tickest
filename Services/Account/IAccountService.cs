using Tickest.Models.Entities;
using Tickest.Models.ViewModels;

namespace Tickest.Services.Authentication
{
    public interface IAccountService
    {
        Task<Usuario> GetCurrentUserAsync();

        Task<ServiceResult> RegisterAsync(UsuarioRegisterViewModel model);

        Task<bool> CreateUserWithRoleAsync(string email, string password, string role);

        Task<ServiceResult> LoginAsync(LoginViewModel loginModel);
    }
}
