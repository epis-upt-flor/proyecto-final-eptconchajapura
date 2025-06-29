using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoCF.Models;
using ProyectoCF.Models.DTO;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoCF.Controllers
{
    public class JuegosrecreativosController : Controller
    {
        private readonly Connection _context;

        public JuegosrecreativosController(Connection context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var juegos = _context.Juegosrecreativos.ToList();
            return View(juegos);
        }

        public IActionResult Create()
        {
            ViewBag.Cursos = new SelectList(_context.Cursos.ToList(), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Juegosrecreativos juego)
        {
            if (ModelState.IsValid)
            {
                juego.Tipo = "Sopa de Letras";
                _context.Juegosrecreativos.Add(juego);
                _context.SaveChanges();

                // Procesar palabras desde Contenido (si no es nulo)
                if (!string.IsNullOrWhiteSpace(juego.Contenido))
                {
                    var palabras = juego.Contenido
                        .Split(new[] { ',', '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries)
                        .Select(p => p.Trim().ToUpper())
                        .Where(p => !string.IsNullOrWhiteSpace(p))
                        .ToList();

                    foreach (var palabra in palabras)
                    {
                        _context.PreguntasJuego.Add(new PreguntasJuego
                        {
                            JuegoId = juego.Id,
                            Palabra = palabra
                        });
                    }

                    _context.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            ViewBag.Cursos = new SelectList(_context.Cursos.ToList(), "Id", "Nombre", juego.CursoId);
            return View(juego);
        }

        public IActionResult Sopa(int id)
        {
            var juego = _context.Juegosrecreativos.FirstOrDefault(j => j.Id == id);
            if (juego == null)
                return NotFound();

            var palabras = _context.PreguntasJuego
                .Where(p => p.JuegoId == id)
                .Select(p => p.Palabra.ToUpper())
                .ToList();

            ViewBag.Palabras = palabras;
            ViewBag.JuegoId = id;

            return View();
        }
        ////
        ////[HttpPost]
        //[Route("api/resultados/sopa")]
        //public IActionResult RegistrarResultadoSopa([FromBody] ResultadoSopaDTO resultado)
        //{
        //    if (resultado == null || resultado.PalabrasEncontradas == null || resultado.UsuarioId <= 0)
        //        return BadRequest("Datos incompletos.");

        //    var entidad = new RespuestasJuegos
        //    {
        //        JuegoId = resultado.JuegoId,
        //        UsuarioId = resultado.UsuarioId,
        //        TiempoSegundos = resultado.TiempoEnSegundos,
        //        Puntaje = resultado.PalabrasEncontradas.Count, // 1 punto por palabra encontrada
        //        FechaRespuesta = DateTime.Now
        //    };

        //    _context.RespuestasJuegos.Add(entidad);
        //    _context.SaveChanges();

        //    return Ok(new { mensaje = "Resultado registrado exitosamente." });
        //}
        ////
    }
}
