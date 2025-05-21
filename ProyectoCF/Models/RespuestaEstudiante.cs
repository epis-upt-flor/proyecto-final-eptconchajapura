using System.ComponentModel.DataAnnotations;

namespace ProyectoCF.Models
{
    public class RespuestaEstudiante
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EstudianteId { get; set; }

        [Required]
        public int EvaluacionId { get; set; }

        [Required]
        public int PreguntaId { get; set; }

        [Required]
        public int RespuestaId { get; set; }

        [Required]
        public bool EsCorrecta { get; set; }

        public DateTime FechaRespuesta { get; set; } = DateTime.Now;

        public Usuario Estudiante { get; set; }
        public Evaluacion Evaluacion { get; set; }
        public Pregunta Pregunta { get; set; }
        public Respuesta Respuesta { get; set; }
    }
}