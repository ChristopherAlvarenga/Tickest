using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tickest.Models.Entities
{
    public class Anexo
    {
        [Key]
        public int Id { get; set; }

        [StringLength(500)]
        public string? Endereco { get; set; }

        [NotMapped]
        public IFormFile? File { get; set; }

        public int? TicketId { get; set; }
        [ForeignKey(nameof(TicketId))]
        public virtual Ticket? Ticket { get; set; }
    }
}
