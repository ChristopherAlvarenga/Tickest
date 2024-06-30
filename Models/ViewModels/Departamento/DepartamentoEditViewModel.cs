using System.ComponentModel.DataAnnotations;
using Tickest.Models.ViewModels;

namespace Tickest.Models.ViewModels
{
    public class DepartamentoEditViewModel
    {
        public DepartamentoEditViewModel()
        {
            GerenciadoresDisponiveis = new List<GerenciadorViewModel>();
            AnalistasDisponiveis = new List<AnalistaViewModel>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Gerenciador obrigatório")]
        public int? GerenciadorSelecionado { get; set; }

        [Required(ErrorMessage = "Analista obrigatório")]
        public int AnalistaSelecionado { get; set; }

        public ICollection <AnalistaViewModel> AnalistasDisponiveis { get; set; }
        public ICollection<GerenciadorViewModel> GerenciadoresDisponiveis { get; set; }
    }
}
