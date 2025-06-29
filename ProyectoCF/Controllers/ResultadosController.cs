using Microsoft.AspNetCore.Mvc;
using ProyectoCF.Models;
using ProyectoCF.Models.DTO;
using System;
using System.Threading.Tasks;

namespace ProyectoCF.Controllers
{
    [ApiController]
    [Route("api/resultados")]
    public class ResultadosController : ControllerBase
    {
        private readonly Connection _context;

        public ResultadosController(Connection context)
        {
            _context = context;
        }

        [HttpPost("sopa")]
        public async Task<IActionResult> RegistrarResultado([FromBody] ResultadoSopaDTO dto)
        {
            try
            {
                var respuesta = new RespuestasJuegos
                {
                    JuegoId = dto.JuegoId,
                    UsuarioId = dto.UsuarioId,
                    TiempoSegundos = dto.TiempoEnSegundos,
                    Puntaje = dto.PalabrasEncontradas.Count,
                    FechaRespuesta = DateTime.Now
                };

                _context.RespuestasJuegos.Add(respuesta);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Guardado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
