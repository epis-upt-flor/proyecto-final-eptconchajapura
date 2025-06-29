using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoCF.Models
{
    public class RespuestaJuego
    {
        [Key]
        public int Id { get; set; }
        public int JuegoId { get; set; }
        public int UsuarioId { get; set; }
        public int TiempoSegundos { get; set; }
        public int Puntaje { get; set; }
        public DateTime FechaRespuesta { get; set; }
    }
}
