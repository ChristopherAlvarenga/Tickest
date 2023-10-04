﻿using System.ComponentModel.DataAnnotations;

namespace Tickest.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(250)]
        public string Senha { get; set; }

        public int CargoId { get; set; }
        public virtual Cargo Cargo { get; set; }

        public int DepartamentoId { get; set; }
        public virtual Departamento Departamento { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
