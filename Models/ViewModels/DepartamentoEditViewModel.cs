using System.ComponentModel.DataAnnotations;

namespace Tickest.Models.ViewModels
{
    public class DepartamentoEditViewModel
    {
        public DepartamentoEditViewModel()
        {
            ResponsaveisDisponiveis = new List<ResponsavelViewModel>();
        }

        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public int ResponsavelSelecionado { get; set; }

        public ICollection<ResponsavelViewModel> ResponsaveisDisponiveis { get; set; }
    }
}
