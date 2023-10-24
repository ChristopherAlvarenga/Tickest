using System.ComponentModel.DataAnnotations;

namespace Tickest.Models.Entities
{
    public class Anexo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string? Endereco { get; set; }

        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}
