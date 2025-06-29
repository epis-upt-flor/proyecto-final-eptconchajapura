using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCF.Models
{
    public class ResultadoSopa
    {
        [Key]
        public int Id { get; set; }

        public int JuegoId { get; set; }

        [Required]
        public string PalabrasEncontradas { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.Now;

        public int DuracionSegundos { get; set; }

        [ForeignKey("JuegoId")]
        public Juegosrecreativos Juego { get; set; }
    }
}
