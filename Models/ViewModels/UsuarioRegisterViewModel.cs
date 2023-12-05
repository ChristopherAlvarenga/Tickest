using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Tickest.Models.ViewModels
{
    public class UsuarioRegisterViewModel
    {
        public UsuarioRegisterViewModel()
        {
            Departamentos = new List<DepartamentoViewModel>();
            Areas = new List<AreaViewModel>();
        }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confimar Senha")]
        [Compare("Senha", ErrorMessage = "As Senhas não condizem")]
        public string ConfirmarSenha { get; set; }

        [Required(ErrorMessage = "Selecione um departamento")]
        public int DepartamentoSelecionado { get; set; }

        [Required(ErrorMessage = "Selecione uma área")]
        public int AreaSelecionada { get; set; }

        public ICollection<DepartamentoViewModel> Departamentos { get; set; }
        public ICollection<AreaViewModel> Areas { get; set; }

        public UsuarioRoleViewModel UserRole { get; set; }

        //public bool Gerenciador { get; set; }

        //public bool Responsavel { get; set; }

        //public string GetRole()
        //{
        //    if (Gerenciador)
        //        return "Gerenciador";
        //    if (Responsavel)
        //        return "Responsavel";

        //    return "Colaborador";
        //}
    }
}
