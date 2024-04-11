using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tickest.Models.Entities
{
    public class Usuario : IdentityUser<int>
    {
        [Required]
        [MaxLength(100)]
        public string? Nome { get; set; }
         
        public int? DepartamentoId { get; set; }
        [ForeignKey(nameof(DepartamentoId))]
        public virtual Departamento? Departamento { get; set; }

        public int? AreaId { get; set; }
        [ForeignKey(nameof(AreaId))]
        public virtual Area? Area { get; set; }

        public ICollection<Ticket> TicketsSolicitados { get; set; }
        public ICollection<Ticket> TicketsResponsaveis { get; set; }

        public ICollection<Notificacao> Notificacoes { get; set; }

        //public static implicit operator Usuario?(IdentityUser? v)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
