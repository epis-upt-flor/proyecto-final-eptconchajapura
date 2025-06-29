using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoCF.Models;

namespace ProyectoCF.Controllers
{
    public class EstudiantesCursosController : Controller
    {
        private readonly Connection _context;

        public EstudiantesCursosController(Connection context)
        {
            _context = context;
        }

        // GET: /EstudiantesCursos
        public IActionResult Index()
        {
            var asignaciones = _context.EstudiantesCursos
                .Include(ec => ec.Estudiante)
                .Include(ec => ec.Curso)
                .ToList();

            return View(asignaciones);
        }

        // GET: /EstudiantesCursos/Create
        public IActionResult Create()
        {
            ViewBag.Estudiantes = _context.Usuarios
                .Where(u => u.Rol == "Estudiante")
                .ToList();

            ViewBag.Cursos = _context.Cursos.ToList();

            return View();
        }


        // POST: /EstudiantesCursos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EstudianteCurso estudianteCurso)
        {
            if (ModelState.IsValid)
            {
                _context.EstudiantesCursos.Add(estudianteCurso);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Estudiantes = _context.Usuarios.Where(u => u.Rol == "Estudiante").ToList();
            ViewBag.Cursos = _context.Cursos.ToList();
            return View(estudianteCurso);
        }



        // GET: /EstudiantesCursos/Delete/5
        public IActionResult Delete(int id)
        {
            var asignacion = _context.EstudiantesCursos
                .Include(ec => ec.Estudiante)
                .Include(ec => ec.Curso)
                .FirstOrDefault(ec => ec.Id == id);

            if (asignacion == null)
            {
                return NotFound();
            }

            return View(asignacion);
        }

        // POST: /EstudiantesCursos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, IFormCollection form)
        {
            var asignacion = _context.EstudiantesCursos.Find(id);

            if (asignacion != null)
            {
                _context.EstudiantesCursos.Remove(asignacion);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
