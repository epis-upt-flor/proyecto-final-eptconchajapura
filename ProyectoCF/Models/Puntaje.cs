using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProyectoCF.Models
{
    public class Puntaje
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(0, 100)]
        public int Valor { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}
