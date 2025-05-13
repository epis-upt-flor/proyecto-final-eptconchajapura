using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoCF.Models;
using ProyectoCF.Services;

namespace ProyectoCF.Controllers
{
    public class MaterialesController : Controller
    {
        private readonly Connection _context;
        private readonly TelegramService _telegramService;

        public MaterialesController(Connection context, TelegramService telegramService)
        {
            _context = context;
            _telegramService = telegramService;
        }

        public IActionResult Index()
        {
            var rol = HttpContext.Session.GetString("Rol");
            var usuarioId = int.Parse(HttpContext.Session.GetString("UsuarioId"));

            List<Material> materiales;
            List<Entregable> entregas;

            if (rol == "Docente")
            {
                // Traemos los materiales del docente en memoria
                materiales = _context.Materiales
                    .Include(m => m.Curso)
                    .Where(m => m.Curso.DocenteId == usuarioId)
                    .ToList();

                // Traemos todas las entregas
                entregas = _context.Entregables
                    .Include(e => e.Usuario)
                    .ToList();
            }
            else if (rol == "Administrador")
            {
                // Traemos todos los materiales en memoria para el administrador
                materiales = _context.Materiales
                    .Include(m => m.Curso)
                    .ToList();

                // Traemos todas las entregas
                entregas = _context.Entregables
                    .Include(e => e.Usuario)
                    .ToList();
            }
            else
            {
                return Forbid();
            }

            // Obtenemos los IDs de los materiales
            var materialIds = materiales.Select(m => m.Id).ToList();

            // Filtramos las entregas en memoria
            var entregasFiltradas = entregas
                .Where(e => materialIds.Contains(e.MaterialId))  // Se filtra en memoria
                .GroupBy(e => e.MaterialId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Pasamos las entregas filtradas a la vista
            ViewBag.EntregasPorMaterial = entregasFiltradas;

            // Pasamos los materiales a la vista
            return View(materiales);
        }



        public IActionResult Create()
        {
            var rol = HttpContext.Session.GetString("Rol");
            var usuarioId = int.Parse(HttpContext.Session.GetString("UsuarioId"));

            if (rol == "Docente")
            {
                ViewBag.Cursos = _context.Cursos.Where(c => c.DocenteId == usuarioId).ToList();
                return View();
            }

            if (rol == "Administrador")
            {
                ViewBag.Cursos = _context.Cursos.ToList();
                return View();
            }

            return Forbid();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Material material)
        {
                _context.Materiales.Add(material);
                await _context.SaveChangesAsync();

                var message = $"📦 Material agregado: {material.Titulo}";
                await _telegramService.SendMessageAsync(message);

                return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var rol = HttpContext.Session.GetString("Rol");
            var usuarioId = int.Parse(HttpContext.Session.GetString("UsuarioId"));

            var material = _context.Materiales.Include(m => m.Curso).FirstOrDefault(m => m.Id == id);

            if (rol == "Docente" && material?.Curso.DocenteId != usuarioId)
            {
                return Forbid();
            }

            if (material == null)
            {
                return NotFound();
            }

            var cursos = _context.Cursos.ToList();

            if (!cursos.Any())
            {
                Console.WriteLine("No se encontraron cursos para el usuario.");
            }
            else
            {
                Console.WriteLine($"Se encontraron {cursos.Count} cursos para el usuario.");
            }

            ViewBag.Cursos = cursos;

            return View(material);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Material material)
        {
            _context.Materiales.Update(material);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var material = _context.Materiales.Find(id);
            if (material == null)
            {
                return NotFound();
            }
            return View(material);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var material = _context.Materiales.Find(id);
            if (material != null)
            {
                _context.Materiales.Remove(material);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
