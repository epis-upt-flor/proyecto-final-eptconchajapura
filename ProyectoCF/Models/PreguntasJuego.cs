using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCF.Models
{
    [Table("PreguntasJuego")]
    public class PreguntasJuego
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int JuegoId { get; set; }

        [Required]
        [StringLength(50)]
        public string Palabra { get; set; }

        [ForeignKey("JuegoId")]
        public Juegosrecreativos Juego { get; set; }
    }
}
