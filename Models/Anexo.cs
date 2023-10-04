using System.ComponentModel.DataAnnotations;

namespace Tickest.Models
{
    public class Anexo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public required string Endereco { get; set; }

        public int TicketId { get; set; }
        public required virtual Ticket Ticket { get; set; }
    }
}
