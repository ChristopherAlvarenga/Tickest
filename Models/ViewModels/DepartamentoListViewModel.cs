namespace Tickest.Models.ViewModels
{
    public class DepartamentoListViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public ResponsavelViewModel Responsavel { get; set; }
    }
}
