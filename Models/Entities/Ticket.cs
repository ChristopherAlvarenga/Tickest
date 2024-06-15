using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tickest.Enums;

namespace Tickest.Models.Entities
{
    #region Ticket Entity

    // Entidade Ticket representa um ticket de suporte ou tarefa.
    public class Ticket
    {
        // Construtor padrão da classe Ticket.
        public Ticket()
        {
            Anexos = new List<Anexo>();
        }

        #region Properties

        // Identificador único do ticket.
        [Key]
        public int Id { get; set; }

        // Título do ticket, obrigatório e com no máximo 100 caracteres.
        [Required(ErrorMessage = "O título do ticket é obrigatório.")]
        [StringLength(100, ErrorMessage = "O título do ticket não pode ter mais de 100 caracteres.")]
        public string Titulo { get; set; }

        // Descrição do ticket, obrigatória e com no máximo 1000 caracteres.
        [Required(ErrorMessage = "A descrição do ticket é obrigatória.")]
        [StringLength(1000, ErrorMessage = "A descrição do ticket não pode ter mais de 1000 caracteres.")]
        public string Descricao { get; set; }

        // Data de criação do ticket, obrigatória.
        [Required(ErrorMessage = "A data de criação do ticket é obrigatória.")]
        public DateTime DataCriacao { get; set; }

        // Data do status do ticket, obrigatória.
        [Required(ErrorMessage = "A data do status do ticket é obrigatória.")]
        public DateTime DataStatus { get; set; }

        // Prioridade do ticket, obrigatória.
        [Required(ErrorMessage = "A prioridade do ticket é obrigatória.")]
        [EnumDataType(typeof(TicketPrioridadeEnum), ErrorMessage = "A prioridade do ticket é inválida.")]
        public TicketPrioridadeEnum Prioridade { get; set; }

        // Status do ticket, obrigatório.
        [Required(ErrorMessage = "O status do ticket é obrigatório.")]
        [EnumDataType(typeof(TicketStatusEnum), ErrorMessage = "O status do ticket é inválido.")]
        public TicketStatusEnum Status { get; set; }

        // ID do departamento ao qual o ticket está associado, opcional.
        public int? DepartamentoId { get; set; }

        // Departamento ao qual o ticket está associado, opcional.
        [ForeignKey(nameof(DepartamentoId))]
        public virtual Departamento? Departamento { get; set; }

        // ID da especialidade associada ao ticket, opcional.
        public int? EspecialidadeId { get; set; }

        // Especialidade associada ao ticket, opcional.
        [ForeignKey(nameof(EspecialidadeId))]
        public virtual Especialidade? Especialidade { get; set; }

        // ID do usuário que solicitou o ticket, obrigatório.
        public int SolicitanteId { get; set; }

        // Usuário que solicitou o ticket, obrigatório.
        [ForeignKey(nameof(SolicitanteId))]
        public Usuario Solicitante { get; set; }

        // ID do usuário responsável pelo ticket, opcional.
        public int? ResponsavelId { get; set; }

        // Usuário responsável pelo ticket, opcional.
        [ForeignKey(nameof(ResponsavelId))]
        public virtual Usuario? Responsavel { get; set; }

        // Lista de anexos associados ao ticket.
        public List<Anexo> Anexos { get; set; }

        #endregion
    }

    #endregion
}
