using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCF.Models
{
    public class Entregable
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MaterialId { get; set; }

        [ForeignKey("MaterialId")]
        public Material Material { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        [Required]
        [StringLength(255)]
        public string NombreArchivo { get; set; }

        [Required]
        [StringLength(100)]
        public string ContentType { get; set; }

        [Required]
        public byte[] ArchivoContenido { get; set; }

        public DateTime FechaEntrega { get; set; } = DateTime.Now;
    }
}