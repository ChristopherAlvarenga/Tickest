using System.ComponentModel.DataAnnotations;

namespace Tickest.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Título { get; set; }

        [Required]
        [StringLength(500)]
        public string Descrição { get; set; }

        [Required]
        [StringLength(500)]
        public string Comentario { get; set; }

        [Required]
        public DateTime Data_Criação { get; set; }

        public DateTime Data_Limite { get; set; }

        public enum Prioridade
        {
            Baixa = 0, Média = 1, Alta = 2, Urgente = 3
        }

        public enum Status
        {
            Análise = 0, Andamento = 1, Teste = 2, Concluído = 3, Cancelado = 4 
        }

        public int SolicitanteId { get; set; }
        public Usuario Solicitante { get; set; }

        public int DestinatarioId { get; set; }
        public Usuario Destinatario { get; set; }
    }
}
