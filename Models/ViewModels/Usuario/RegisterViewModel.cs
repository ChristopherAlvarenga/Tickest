using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tickest.Models.ViewModels.Funcao;

namespace Tickest.Models.ViewModels
{
    /// <summary>
    /// ViewModel para registro de usuário.
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// Construtor padrão que inicializa as coleções.
        /// </summary>
        public RegisterViewModel()
        {
            OpcoesFuncoes = new List<FuncaoViewModel>();
            OpcoesDepartamentos = new List<DepartamentoListViewModel>();
            OpcoesEspecialidades = new List<EspecialidadeViewModel>();
        }

        /// <summary>
        /// Nome do usuário (obrigatório, máximo de 100 caracteres).
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        /// <summary>
        /// Email do usuário (obrigatório).
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Senha do usuário (obrigatória).
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        /// <summary>
        /// Confirmação de senha (comparação com Senha).
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha")]
        [Compare("Senha", ErrorMessage = "As senhas não coincidem.")]
        public string ConfirmarSenha { get; set; }

        /// <summary>
        /// ID da função selecionada (obrigatório).
        /// </summary>
        [Required(ErrorMessage = "Selecione uma atribuição")]
        public int FuncaoId { get; set; }

        /// <summary>
        /// ID do departamento selecionado.
        /// </summary>
        public int? DepartamentoId { get; set; }

        /// <summary>
        /// ID da especialidade selecionada.
        /// </summary>
        public int? EspecialidadeId { get; set; }

        /// <summary>
        /// Opções de funções disponíveis para seleção.
        /// </summary>
        public IEnumerable<FuncaoViewModel> OpcoesFuncoes { get; set; }

        /// <summary>
        /// Opções de departamentos disponíveis para seleção.
        /// </summary>
        public IEnumerable<DepartamentoListViewModel> OpcoesDepartamentos { get; set; }

        /// <summary>
        /// Opções de especialidades disponíveis para seleção.
        /// </summary>
        public IEnumerable<EspecialidadeViewModel> OpcoesEspecialidades { get; set; }
    }
}
