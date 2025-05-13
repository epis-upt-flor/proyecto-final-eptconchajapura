using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoCF.Models;
using System.IO;

namespace ProyectoCF.Controllers
{
    public class EntregablesController : Controller
    {
        private readonly Connection _context;
        private readonly ILogger<EntregablesController> _logger;

        public EntregablesController(Connection context, ILogger<EntregablesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult CheckExisting(int materialId)
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
                return Json(new { exists = false });

            var usuarioId = int.Parse(usuarioIdStr);
            var exists = _context.Entregables
                .Any(e => e.MaterialId == materialId && e.UsuarioId == usuarioId);

            return Json(new { exists });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(int materialId, IFormFile archivo)
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                TempData["ErrorMessage"] = "Usuario no autenticado.";
                return RedirectToAction("Index", "Cursos");
            }

            var usuarioId = int.Parse(usuarioIdStr);

            var cursoId = _context.Materiales
                .Where(m => m.Id == materialId)
                .Select(m => m.CursoId)
                .FirstOrDefault();

            if (archivo == null || archivo.Length == 0)
            {
                TempData["ErrorMessage"] = "Por favor selecciona un archivo.";
                return RedirectToAction("Details", "Cursos", new { id = cursoId });
            }

            var extension = Path.GetExtension(archivo.FileName).ToLower();
            if (extension != ".pdf")
            {
                TempData["ErrorMessage"] = "Solo se permiten archivos PDF.";
                return RedirectToAction("Details", "Cursos", new { id = cursoId });
            }

            try
            {
                using var memoryStream = new MemoryStream();
                await archivo.CopyToAsync(memoryStream);

                var entregable = new Entregable
                {
                    MaterialId = materialId,
                    UsuarioId = usuarioId,
                    NombreArchivo = archivo.FileName,
                    ContentType = archivo.ContentType,
                    ArchivoContenido = memoryStream.ToArray()
                };

                var existente = _context.Entregables
                    .FirstOrDefault(e => e.MaterialId == materialId && e.UsuarioId == usuarioId);

                if (existente != null)
                {
                    _context.Entregables.Remove(existente);
                }

                _context.Entregables.Add(entregable);
                await _context.SaveChangesAsync();

                TempData["Success"] = "✅ Tu entrega fue registrada correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir entregable");
                TempData["ErrorMessage"] = "❌ Hubo un error al subir el archivo.";
            }

            return RedirectToAction("Details", "Cursos", new { id = cursoId });
        }


        public async Task<IActionResult> Descargar(int id)
        {
            var entregable = await _context.Entregables.FindAsync(id);

            if (entregable == null)
                return NotFound();

            return File(entregable.ArchivoContenido, entregable.ContentType, entregable.NombreArchivo);
        }

    }
}
