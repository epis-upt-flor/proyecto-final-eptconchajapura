namespace ProyectoCF.Models
{
    public class ViewModels
    {

        public class RespuestasAlumnoViewModel
        {
            public int AlumnoId { get; set; }
            public string NombreCompleto { get; set; }
            public List<DetalleRespuestaViewModel> Respuestas { get; set; }
            public decimal Calificacion { get; set; }
        }

        public class DetalleRespuestaViewModel
        {
            public int PreguntaId { get; set; }
            public string PreguntaTexto { get; set; }
            public int RespuestaId { get; set; }
            public string RespuestaTexto { get; set; }
            public bool EsCorrecta { get; set; }
            public string RespuestaCorrecta { get; set; }
        }
    }
}
