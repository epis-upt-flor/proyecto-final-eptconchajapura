using GenerativeAI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoCF.Models;

namespace ProyectoCF.Controllers
{
    [Route("Chat")]
    public class ChatController : Controller
    {
        private readonly Connection _context;

        public ChatController(Connection context)
        {
            _context = context;
        }

        [HttpGet("Index")]
        public IActionResult Index()
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Estudiante")
            {
                return Forbid();
            }

            var usuarioIdString = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdString)) return Unauthorized();

            var usuarioId = int.Parse(usuarioIdString);
            var cursos = _context.EstudiantesCursos
                .Include(ec => ec.Curso)
                .Where(ec => ec.EstudianteId == usuarioId)
                .Select(ec => ec.Curso)
                .ToList();

            return View(cursos);
        }

        [HttpGet("Chat/{id}")]
        public IActionResult Chat(int id)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Estudiante")
            {
                return Forbid();
            }

            var usuarioIdString = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdString)) return Unauthorized();

            var usuarioId = int.Parse(usuarioIdString);
            var curso = _context.EstudiantesCursos
                .Include(ec => ec.Curso)
                .FirstOrDefault(ec => ec.CursoId == id && ec.EstudianteId == usuarioId)?.Curso;

            if (curso == null)
            {
                return NotFound();
            }

            ViewBag.Curso = curso;
            return View();
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Estudiante")
            {
                return Forbid();
            }

            var usuarioIdString = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdString)) return Unauthorized();

            var usuarioId = int.Parse(usuarioIdString);
            var curso = _context.EstudiantesCursos
                .Include(ec => ec.Curso)
                .FirstOrDefault(ec => ec.CursoId == request.CursoId && ec.EstudianteId == usuarioId)?.Curso;

            if (curso == null)
            {
                return NotFound();
            }

            var cursoContext = $"Curso: {curso.Nombre}\nDescripción: {curso.Descripcion}";

            try
            {
                var apiKey = "AIzaSyDrw9lNttNSHH4uWCACWHj2yvB2U0phvKU";
                var googleAI = new GoogleAi(apiKey);
                var model = googleAI.CreateGenerativeModel("models/gemini-2.0-pro-exp-02-05");
                var prompt = $"{cursoContext}\n\nPregunta: {request.Message}";
                var response = await model.GenerateContentAsync(prompt);
                var aiResponse = response.Text();

                return Json(new { userMessage = request.Message, iaResponse = aiResponse });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al comunicarse con Gemini: {ex.Message}");
                return Json(new { error = "Error interno al comunicarse con la IA." });
            }
        }
    }

    public class ChatRequest
    {
        public int CursoId { get; set; }
        public string Message { get; set; }
    }
}
