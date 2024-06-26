﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Tickest.Models.Entities
{
    public class Notificacao
    {
        public int Id { get; set; }

        public int? UsuarioId { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }
        
        public int? TicketId { get; set; }
        [ForeignKey(nameof(TicketId))]
        public Ticket? Ticket { get; set; }
    }
}
