using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCF.Models
{
    public class RespuestasJuegos
    {
        [Key]
        public int Id { get; set; }

        public int JuegoId { get; set; }

        public int UsuarioId { get; set; }

        public int TiempoSegundos { get; set; }

        public int Puntaje { get; set; }

        public DateTime FechaRespuesta { get; set; } = DateTime.Now;

        [ForeignKey("JuegoId")]
        public Juegosrecreativos Juego { get; set; }
    }
}
