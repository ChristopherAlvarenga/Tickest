using Tickest.Models.ViewModels;

namespace Tickest.Models.ViewModels
{
    public class DepartamentoListViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Gerenciador { get; set; }

        public ICollection<EspecialidadeViewModel> Especialidades { get; set; }
    }
}
