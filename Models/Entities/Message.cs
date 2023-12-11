using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tickest.Models.Entities
{

    [Table("Message")]
    public class Message
    {
        [Key]
        public int id { get; set; }

        [Column(TypeName = "text")]
        public string msg_content { get; set; }
        public int? user_id_from { get; set; }

        [ForeignKey("user_id_from")]
        public virtual Usuario? User_from { get; set; }


        [Column(TypeName = "datetime")]
        public DateTime? dataHora { get; set; }
        public int? visu_status { get; set; }
   
        public int? ticket_id { get; set; }

        [ForeignKey("ticket_id")]
        public virtual Ticket? Ticket { get; set; }

    }
}

