using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Collections.Generic;


namespace ProyectoCF.Models.DTO
{
    public class ResultadoSopaDTO
    {
        public int JuegoId { get; set; }
        public int UsuarioId { get; set; }
        public int TiempoEnSegundos { get; set; }  // ✅ Nombre correcto
        public List<string> PalabrasEncontradas { get; set; }
        // En ResultadoSopaDTO.cs

    }
}
