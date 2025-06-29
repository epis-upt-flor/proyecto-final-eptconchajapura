using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCF.Models
{
    public class Juegosrecreativos
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(100)]
        public string? Titulo { get; set; }

        [Required]
        public string Tipo { get; set; } = "Sopa de Letras";

        [StringLength(250)]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El contenido es obligatorio")]
        public string? Contenido { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un curso")]
        [ForeignKey("Curso")]
        public int CursoId { get; set; }

        public Curso? Curso { get; set; }
    }
}
