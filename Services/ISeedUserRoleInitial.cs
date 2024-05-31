namespace Tickest.Services
{
    public interface ISeedUserRoleInitial
    {
        Task SeedRolesAsync();
        Task SeedUsersAsync();
    }
}
