using Tickest.Models.Entities;

namespace Tickest.Models.ViewModels
{
    public class UsuarioEditViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Cargo { get; set;}

        public int DepartamentoId { get; set; }
        public int AreaId { get; set; }
        public ICollection<Departamento> Departamentos { get; set; }
        public ICollection<Area> Areas { get; set; }
    }
}
