﻿using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;

namespace Tickest.Models.ViewModels
{
    public class RegisterViewModel
    {
        public RegisterViewModel()
        {
            OpcoesFuncoes = new List<FuncaoViewModel>();
        }

        [Required]
        [MaxLength(100)]
        public string? Nome { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Senha { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confimar Senha")]
        [Compare("Senha", ErrorMessage = "As Senhas não condizem")]
        public string ConfirmarSenha { get; set; }

        [Required(ErrorMessage = "Selecione uma atribuição")]
        public int FuncaoId { get; set; }
        public int? DepartamentoId { get; set; }
        public int? EspecialidadeId { get; set; }

        public ICollection<FuncaoViewModel> OpcoesFuncoes { get; set; }
        //public ICollection<DepartamentoListViewModel> OpcoesDepartamentos { get; set; }
        
    }
}
