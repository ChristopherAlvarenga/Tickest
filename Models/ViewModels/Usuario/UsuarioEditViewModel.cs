using System.ComponentModel.DataAnnotations;

namespace Tickest.Models.ViewModels
{
    public class UsuarioEditViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um email válido.")]
        public string Email { get; set; }

        public string NovaSenha { get; set; }

        [Compare("NovaSenha", ErrorMessage = "As senhas não correspondem.")]
        public string ConfirmarNovaSenha { get; set; }

        [Required(ErrorMessage = "Selecione uma função.")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Selecione um departamento.")]
        public int DepartamentoId { get; set; }

        [Required(ErrorMessage = "Selecione uma especialidade.")]
        public int EspecialidadeId { get; set; }
    }
}
