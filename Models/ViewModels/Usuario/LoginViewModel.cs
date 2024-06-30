using System.ComponentModel.DataAnnotations;

namespace Tickest.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A Senha é obrigatória")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Display(Name = "Manter-se conectado")]
        public bool ManterLogin { get; set; }
    }
}
