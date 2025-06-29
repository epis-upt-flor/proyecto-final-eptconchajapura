using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ProyectoCF.Models.DTO
{
    public class ResultadoJuegoDTO
    {
        public int JuegoId { get; set; }
        public int UsuarioId { get; set; }
        public int Tiempo { get; set; }
        public List<string> PalabrasEncontradas { get; set; }
    }
}


