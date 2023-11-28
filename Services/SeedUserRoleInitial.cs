using Microsoft.AspNetCore.Identity;

namespace Tickest.Services
{
    public class SeedUserRoleInitial : ISeedUserRoleInitial
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedUserRoleInitial(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedRolesAsync()
        {
            if(!await _roleManager.RoleExistsAsync("Admin"))
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Admin";
                role.NormalizedName = "ADMIN";
                role.ConcurrencyStamp = Guid.NewGuid().ToString();

                IdentityResult roleResult = await _roleManager.CreateAsync(role);
            }

            if (!await _roleManager.RoleExistsAsync("Gerenciador"))
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Gerenciador";
                role.NormalizedName = "GERENCIADOR";
                role.ConcurrencyStamp = Guid.NewGuid().ToString();

                IdentityResult roleResult = await _roleManager.CreateAsync(role);
            }

            if (!await _roleManager.RoleExistsAsync("Responsavel"))
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Responsavel";
                role.NormalizedName = "RESPONSAVEL";
                role.ConcurrencyStamp = Guid.NewGuid().ToString();

                IdentityResult roleResult = await _roleManager.CreateAsync(role);
            }

            if (!await _roleManager.RoleExistsAsync("Desenvolvedor"))
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Desenvolvedor";
                role.NormalizedName = "DESENVOLVEDOR";
                role.ConcurrencyStamp = Guid.NewGuid().ToString();

                IdentityResult roleResult = await _roleManager.CreateAsync(role);
            }
        }

        public async Task SeedUsersAsync()
        {
            if(await _userManager.FindByEmailAsync("admin@localhost") == null)
            {
                IdentityUser user = new IdentityUser();
                user.UserName = "admin@localhost";
                user.Email = "admin@localhost";
                user.NormalizedUserName = "ADMIN@LOCALHOST";
                user.NormalizedEmail = "ADMIN@LOCALHOST";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                user.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult userResult = await _userManager.CreateAsync(user, "#SecretPass456");

                if(userResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }

            if (await _userManager.FindByEmailAsync("gerenciador@localhost") == null)
            {
                IdentityUser user = new IdentityUser();
                user.UserName = "gerenciador@localhost";
                user.Email = "gerenciador@localhost";
                user.NormalizedUserName = "GERENCIADOR@LOCALHOST";
                user.NormalizedEmail = "GERENCIADOR@LOCALHOST";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                user.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult userResult = await _userManager.CreateAsync(user, "#SecretPass456");

                if (userResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Gerenciador");
                }
            }

            if (await _userManager.FindByEmailAsync("responsavel@localhost") == null)
            {
                IdentityUser user = new IdentityUser();
                user.UserName = "responsavel@localhost";
                user.Email = "responsavel@localhost";
                user.NormalizedUserName = "RESPONSAVEL@LOCALHOST";
                user.NormalizedEmail = "RESPONSAVEL@LOCALHOST";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                user.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult userResult = await _userManager.CreateAsync(user, "#SecretPass456");

                if (userResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Responsavel");
                }
            }

            if (await _userManager.FindByEmailAsync("desenvolvedor@localhost") == null)
            {
                IdentityUser user = new IdentityUser();
                user.UserName = "desenvolvedor@localhost";
                user.Email = "desenvolvedor@localhost";
                user.NormalizedUserName = "DESENVOLVEDOR@LOCALHOST";
                user.NormalizedEmail = "DESENVOLVEDOR@LOCALHOST";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                user.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult userResult = await _userManager.CreateAsync(user, "#SecretPass456");

                if (userResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Desenvolvedor");
                }
            }
        }
    }
}
