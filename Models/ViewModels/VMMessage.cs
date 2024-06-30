using System.ComponentModel.DataAnnotations;

namespace Tickest.Models.ViewModels
{
    public class VMMessage
    {
        [Required(ErrorMessage = "User de Envio obrigatório")]
        public int? from { get; set; }

        [Required(ErrorMessage = "User de Recebimento obrigatório")]
        public int? ticket_id{ get; set; }
        [Required(ErrorMessage = "Mensagem obrigatório")]
        public string msg { get; set; }
    }
}
