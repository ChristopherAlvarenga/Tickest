using System.Collections.Generic;
using Tickest.Models.Entities;

namespace Tickest.Models.ViewModels
{
    public class UsuarioEditViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Funcao { get; set; }

        public int DepartamentoId { get; set; }
        public int EspecialidadeId { get; set; }

        public ICollection<Departamento> Departamentos { get; set; }
        public ICollection<Especialidade> Especialidades { get; set; }
    }
}
