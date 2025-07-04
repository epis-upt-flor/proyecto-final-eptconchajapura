using Microsoft.EntityFrameworkCore;

namespace ProyectoCF.Models
{
    public class Connection : DbContext
    {
        public Connection(DbContextOptions<Connection> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<EstudianteCurso> EstudiantesCursos { get; set; }
        public DbSet<Material> Materiales { get; set; }
        public DbSet<Evaluacion> Evaluaciones { get; set; }
        public DbSet<Nota> Notas { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }
        public DbSet<Pregunta> Preguntas { get; set; }
        public DbSet<Respuesta> Respuestas { get; set; }
        public DbSet<Entregable> Entregables { get; set; }
        public DbSet<RespuestaEstudiante> RespuestasEstudiantes { get; set; }
        public DbSet<Puntaje> Puntajes { get; set; }
        //
        //

        public DbSet<Juegosrecreativos> Juegosrecreativos { get; set; }
        public DbSet<PreguntasJuego> PreguntasJuego { get; set; }
        public DbSet<ResultadoSopa> ResultadosSopa { get; set; }
        public ICollection<PreguntasJuego>? PalabrasJuego { get; set; }

        // Aqu� va tu nueva tabla
        public DbSet<RespuestasJuegos> RespuestasJuegos { get; set; }


    }
}