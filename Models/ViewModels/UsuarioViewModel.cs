﻿using Tickest.Models.Entities;

namespace Tickest.Models.ViewModels
{
    public class UsuarioViewModel
    {
        public Usuario Usuario { get; set; }
        public Departamento Departamento { get; set; }
        public Area Area { get; set; }

        public ICollection<Usuario> Usuarios { get; set; }
        public ICollection<Departamento> Departamentos { get; set; }
    }
}
