using System.ComponentModel.DataAnnotations;

namespace Tickest.Models.ViewModels
{
    public class RegisterViewModel
    {
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

        public bool Gerenciador { get; set; }

        public bool Responsavel { get; set; }

        public string GetRole()
        {
            if (Gerenciador)
                return "Gerenciador";
            if (Responsavel)
                return "Responsavel";

            return "Colaborador";
        }
    }
}
