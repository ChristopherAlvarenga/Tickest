using Tickest.Models.Entities;

namespace Tickest.Models.ViewModels
{
    public class UsuarioDepartamentoViewModel
    {
        public Departamento Departamento { get; set; }
        public ICollection<Usuario> Usuarios { get; set; }
    }
}
