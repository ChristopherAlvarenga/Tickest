namespace Tickest.Models.ViewModels
{
    public class UsuarioRoleViewModel
    {
        public UsuarioRoleViewModel(string id, string nome)
        {
            Id = id;
            Nome = nome;
        }

        public string Id { get; set; }

        public string Nome { get; set; }
    }
}
