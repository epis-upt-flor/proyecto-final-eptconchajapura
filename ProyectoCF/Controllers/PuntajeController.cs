using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Newtonsoft.Json;
using ProyectoCF.Models;

namespace ProyectoCF.Controllers
{
    public class PuntajeController : Controller
    {
        private readonly Connection _context;
        private readonly IWebHostEnvironment _env;

        public PuntajeController(Connection context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public IActionResult VerificarJuegoHoy()
        {
            var userIdString = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized("Usuario no autenticado en sesión.");

            int userId = int.Parse(userIdString);

            var hoy = DateTime.Today;
            var yaJugoHoy = _context.Puntajes
                .Any(p => p.UsuarioId == userId && p.Fecha.Date == hoy);

            return Json(new { yaJugoHoy });
        }



        [HttpGet]
        public IActionResult ObtenerPregunta(string nivel)
        {
            try
            {
                var path = Path.Combine(_env.WebRootPath, "json/preguntas.json");
                var jsonData = System.IO.File.ReadAllText(path);
                var todasPreguntas = JsonConvert.DeserializeObject<List<PreguntaR>>(jsonData);

                var preguntasNivel = todasPreguntas!
                    .Where(p => p.Nivel!.Equals(nivel, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (preguntasNivel.Count == 0)
                    return NotFound("No hay preguntas para este nivel");

                var rnd = new Random();
                var preguntaAleatoria = preguntasNivel[rnd.Next(preguntasNivel.Count)];

                return Json(preguntaAleatoria);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener pregunta: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult GuardarPuntaje([FromBody] int valor)
        {
            var userIdString = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized("Usuario no autenticado en sesión.");

            int userId = int.Parse(userIdString);

            var puntaje = new Puntaje
            {
                Valor = valor,
                Fecha = DateTime.Now,
                UsuarioId = userId
            };

            _context.Puntajes.Add(puntaje);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult ObtenerTopTresUsuarios()
        {
            try
            {
                var topUsuarios = _context.Puntajes
                    .GroupBy(p => p.UsuarioId)
                    .Select(g => new
                    {
                        UsuarioId = g.Key,
                        PuntajeTotal = g.Sum(p => p.Valor)
                    })
                    .Join(_context.Usuarios,
                        puntaje => puntaje.UsuarioId,
                        usuario => usuario.Id,
                        (puntaje, usuario) => new
                        {
                            usuario.Id,
                            usuario.Nombre,
                            puntaje.PuntajeTotal
                        })
                    .OrderByDescending(u => u.PuntajeTotal)
                    .Take(3)
                    .ToList();

                return Json(topUsuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el top de usuarios: {ex.Message}");
            }
        }

    }

    public class PreguntaR
    {
        [JsonProperty("nivel")]
        public string? Nivel { get; set; }

        [JsonProperty("puntos")]
        public int Puntos { get; set; }

        [JsonProperty("texto")]
        public string? Texto { get; set; }

        [JsonProperty("opciones")]
        public OpcionR[]? Opciones { get; set; }
    }

    public class OpcionR
    {
        [JsonProperty("texto")]
        public string? Texto { get; set; }

        [JsonProperty("correcta")]
        public bool Correcta { get; set; }
    }

}