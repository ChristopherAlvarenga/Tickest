using System.ComponentModel.DataAnnotations;

namespace Tickest.Models.ViewModels
{
    public class DepartamentoEditViewModel
    {
        public DepartamentoEditViewModel()
        {
            GerenciadorDisponiveis = new List<GerenciadorViewModel>();
        }

        public int Id { get; set; }

        [Required (ErrorMessage = "Campo Obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Gerenciador obrigatório")]
        public int? GerenciadorSelecionado { get; set; }

        public ICollection<GerenciadorViewModel> GerenciadorDisponiveis { get; set; }
    }
}
