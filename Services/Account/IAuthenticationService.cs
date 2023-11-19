using Tickest.Models.Entities;
using Tickest.Models.ViewModels;

namespace Tickest.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<Usuario> GetCurrentUserAsync();

        Task<ServiceResult> RegisterAsync(RegisterViewModel model);

        Task<bool> CreateUserWithRoleAsync(string email, string password, string role);
        Task<ServiceResult> LoginAsync(LoginViewModel loginModel);
    }
}
