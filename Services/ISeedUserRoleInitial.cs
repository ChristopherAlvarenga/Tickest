namespace Tickest.Services
{
    public interface ISeedUserRoleInitial
    {
        Task SeedCargoAsync();
        Task SeedRolesAsync();
        Task SeedUsersAsync();
    }
}
