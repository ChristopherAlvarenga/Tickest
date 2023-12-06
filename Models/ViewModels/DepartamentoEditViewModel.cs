namespace Tickest.Models.ViewModels
{
    public class DepartamentoEditViewModel
    {
        public DepartamentoEditViewModel()
        {
            ResponsaveisDisponiveis = new List<ResponsavelViewModel>();
        }

        public int Id { get; set; }

        public string Nome { get; set; }

        public int ResponsavelSelecionado { get; set; }

        public ICollection<ResponsavelViewModel> ResponsaveisDisponiveis { get; set; }
    }
}
