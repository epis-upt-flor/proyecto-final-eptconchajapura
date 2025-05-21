using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoCF.Models;
using ProyectoCF.Services;
using static ProyectoCF.Models.ViewModels;

namespace ProyectoCF.Controllers
{
    public class EvaluacionesController : Controller
    {
        private readonly Connection _context;
        private readonly TelegramService _telegramService;

        public EvaluacionesController(Connection context, TelegramService telegramService)
        {
            _context = context;
            _telegramService = telegramService;
        }

        public IActionResult Index()
        {
            var usuarioId = int.Parse(HttpContext.Session.GetString("UsuarioId"));
            var rol = HttpContext.Session.GetString("Rol");
            ViewBag.Rol = rol;

            List<EvaluacionViewModel> evaluacionesViewModel = new List<EvaluacionViewModel>();

            if (rol == "Administrador")
            {
                evaluacionesViewModel = _context.Evaluaciones
                    .Include(e => e.Curso)
                    .Select(e => new EvaluacionViewModel
                    {
                        Id = e.Id,
                        Titulo = e.Titulo,
                        Descripcion = e.Descripcion,
                        CursoNombre = e.Curso.Nombre,
                        YaRendida = false
                    })
                    .ToList();
            }
            else if (rol == "Docente")
            {
                evaluacionesViewModel = _context.Evaluaciones
                    .Include(e => e.Curso)
                    .Where(e => e.Curso.DocenteId == usuarioId)
                    .Select(e => new EvaluacionViewModel
                    {
                        Id = e.Id,
                        Titulo = e.Titulo,
                        Descripcion = e.Descripcion,
                        CursoNombre = e.Curso.Nombre,
                        YaRendida = false
                    })
                    .ToList();
            }
            else if (rol == "Estudiante")
            {
                var evaluaciones = (from e in _context.Evaluaciones.Include(e => e.Curso)
                                    join ec in _context.EstudiantesCursos
                                      on e.CursoId equals ec.CursoId
                                    where ec.EstudianteId == usuarioId
                                    select e)
                                    .Distinct()
                                    .ToList();

                evaluacionesViewModel = evaluaciones.Select(e => new EvaluacionViewModel
                {
                    Id = e.Id,
                    Titulo = e.Titulo,
                    Descripcion = e.Descripcion,
                    CursoNombre = e.Curso.Nombre,
                    YaRendida = _context.Notas.Any(n => n.EvaluacionId == e.Id && n.EstudianteId == usuarioId)
                }).ToList();
            }

            return View(evaluacionesViewModel);
        }

        public IActionResult Create()
        {
            var rol = HttpContext.Session.GetString("Rol");
            var usuarioId = int.Parse(HttpContext.Session.GetString("UsuarioId"));

            if (rol != "Docente" && rol != "Administrador")
            {
                return Forbid();
            }

            ViewBag.Cursos = rol == "Administrador"
                ? _context.Cursos.ToList()
                : _context.Cursos.Where(c => c.DocenteId == usuarioId).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Evaluacion evaluacion)
        {
            _context.Evaluaciones.Add(evaluacion);
            await _context.SaveChangesAsync();
            var message = $"📦 Evaluación creada: {evaluacion.Titulo}";
            await _telegramService.SendMessageAsync(message);
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Details(int id)
        {
            var evaluacion = _context.Evaluaciones
                .Include(e => e.Curso)
                .Include(e => e.Preguntas)
                    .ThenInclude(p => p.Respuestas)
                .FirstOrDefault(e => e.Id == id);

            if (evaluacion == null)
            {
                return NotFound();
            }

            var rol = HttpContext.Session.GetString("Rol");

            if (rol == "Docente" || rol == "Administrador")
            {
                var respuestas = _context.RespuestasEstudiantes
                    .Where(re => re.EvaluacionId == id)
                    .Include(re => re.Estudiante)
                    .Include(re => re.Pregunta)
                    .Include(re => re.Respuesta)
                    .ToList();

                var notas = _context.Notas
                    .Where(n => n.EvaluacionId == id)
                    .ToList();

                var respuestasAlumnos = respuestas
                    .GroupBy(re => re.Estudiante)
                    .Select(g => new RespuestasAlumnoViewModel
                    {
                        AlumnoId = g.Key.Id,
                        NombreCompleto = $"{g.Key.Nombre} {g.Key.Apellido}",
                        Respuestas = g.Select(r => new DetalleRespuestaViewModel
                        {
                            PreguntaId = r.PreguntaId,
                            PreguntaTexto = r.Pregunta.Texto,
                            RespuestaId = r.RespuestaId,
                            RespuestaTexto = r.Respuesta.Texto,
                            EsCorrecta = r.EsCorrecta,
                            RespuestaCorrecta = r.Pregunta.Respuestas.FirstOrDefault(resp => resp.EsCorrecta)?.Texto ?? "No definida"
                        }).OrderBy(r => r.PreguntaId).ToList(),
                        Calificacion = notas.FirstOrDefault(n => n.EstudianteId == g.Key.Id)?.Calificacion ?? 0
                    })
                    .OrderBy(a => a.NombreCompleto)
                    .ToList();

                ViewData["RespuestasAlumnos"] = respuestasAlumnos;
            }

            return View(evaluacion);
        }

        public IActionResult Rendir(int id)
        {
            var evaluacion = _context.Evaluaciones
                .Include(e => e.Preguntas)
                    .ThenInclude(p => p.Respuestas)
                .FirstOrDefault(e => e.Id == id);

            if (evaluacion == null) return NotFound();

            var model = new RendirEvaluacionViewModel
            {
                EvaluacionId = evaluacion.Id,
                Titulo = evaluacion.Titulo,
                Preguntas = evaluacion.Preguntas.Select(q => new PreguntaRespuestaViewModel
                {
                    PreguntaId = q.Id,
                    Texto = q.Texto,
                    Respuestas = q.Respuestas.Select(r => new RespuestaOption
                    {
                        RespuestaId = r.Id,
                        Texto = r.Texto
                    }).ToList()
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rendir(RendirEvaluacionViewModel model)
        {
            var estudianteId = int.Parse(HttpContext.Session.GetString("UsuarioId"));

            var estudiante = _context.Usuarios.FirstOrDefault(u => u.Id == estudianteId);
            if (estudiante == null)
            {
                return NotFound("Estudiante no encontrado.");
            }

            var evaluacion = _context.Evaluaciones.FirstOrDefault(e => e.Id == model.EvaluacionId);
            if (evaluacion == null)
            {
                return NotFound("Evaluación no encontrada.");
            }

            int totalPreguntas = model.Preguntas.Count;
            int respuestasCorrectas = 0;
            var respuestasEstudiante = new List<RespuestaEstudiante>();

            foreach (var preguntaVM in model.Preguntas)
            {
                var respuestaSeleccionada = _context.Respuestas
                    .FirstOrDefault(r => r.Id == preguntaVM.RespuestaSeleccionada);

                var respuestaCorrecta = _context.Respuestas
                    .Where(r => r.PreguntaId == preguntaVM.PreguntaId && r.EsCorrecta)
                    .FirstOrDefault();

                bool esCorrecta = respuestaSeleccionada?.Id == respuestaCorrecta?.Id;

                if (esCorrecta)
                {
                    respuestasCorrectas++;
                }

                var respuestaEstudiante = new RespuestaEstudiante
                {
                    EstudianteId = estudianteId,
                    EvaluacionId = model.EvaluacionId,
                    PreguntaId = preguntaVM.PreguntaId,
                    RespuestaId = preguntaVM.RespuestaSeleccionada ?? 0,
                    EsCorrecta = esCorrecta
                };
                if (preguntaVM.RespuestaSeleccionada == null || preguntaVM.RespuestaSeleccionada == 0)
                {
                    continue;
                }

                respuestasEstudiante.Add(respuestaEstudiante);
            }

            _context.RespuestasEstudiantes.AddRange(respuestasEstudiante);

            double porcentaje = (double)respuestasCorrectas / totalPreguntas * 100;

            var nota = new Nota
            {
                EstudianteId = estudianteId,
                EvaluacionId = model.EvaluacionId,
                Calificacion = (decimal)porcentaje
            };

            _context.Notas.Add(nota);
            await _context.SaveChangesAsync();

            string estado = porcentaje >= 60 ? "✅ Aprobado" : "❌ Reprobado";
            string mensajeTelegram = $"🎓 {estudiante.Nombre} {estudiante.Apellido} ha rendido: 📚 {evaluacion.Titulo}\n" +
                                    $"📊 Puntaje: {porcentaje:F2}%\n" +
                                    $"🎯 Estado: {estado}";

            await _telegramService.SendMessageAsync(mensajeTelegram);

            ViewBag.Resultado = $"Respondiste correctamente {respuestasCorrectas} de {totalPreguntas} preguntas. Tu puntaje es {porcentaje:F2}%.";

            return View("Resultado");
        }

        public IActionResult ObtenerRespuestasAlumnos(int evaluacionId)
        {
            var rol = HttpContext.Session.GetString("Rol");

            if (rol != "Docente" && rol != "Administrador")
            {
                return Forbid();
            }

            var respuestas = _context.RespuestasEstudiantes
                .Where(re => re.EvaluacionId == evaluacionId)
                .Include(re => re.Estudiante)
                .Include(re => re.Pregunta)
                .Include(re => re.Respuesta)
                .GroupBy(re => re.Estudiante)
                .Select(g => new RespuestasAlumnoViewModel
                {
                    AlumnoId = g.Key.Id,
                    NombreCompleto = $"{g.Key.Nombre} {g.Key.Apellido}",
                    Respuestas = g.Select(r => new DetalleRespuestaViewModel
                    {
                        PreguntaTexto = r.Pregunta.Texto,
                        RespuestaTexto = r.Respuesta.Texto,
                        EsCorrecta = r.EsCorrecta,
                        RespuestaCorrecta = _context.Respuestas
                            .FirstOrDefault(resp => resp.PreguntaId == r.PreguntaId && resp.EsCorrecta).Texto
                    }).ToList(),
                    Calificacion = _context.Notas
                        .FirstOrDefault(n => n.EvaluacionId == evaluacionId && n.EstudianteId == g.Key.Id).Calificacion
                })
                .ToList();

            return PartialView("_RespuestasAlumnosModal", respuestas);
        }

        public IActionResult Delete(int id)
        {
            var evaluacion = _context.Evaluaciones
                .Include(e => e.Curso)
                .FirstOrDefault(e => e.Id == id);

            if (evaluacion == null)
            {
                return NotFound();
            }

            return View(evaluacion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var evaluacion = _context.Evaluaciones.Find(id);

            if (evaluacion != null)
            {
                var preguntas = _context.Preguntas.Where(p => p.EvaluacionId == id).ToList();
                foreach (var pregunta in preguntas)
                {
                    var respuestas = _context.Respuestas.Where(r => r.PreguntaId == pregunta.Id).ToList();
                    _context.Respuestas.RemoveRange(respuestas);
                }
                _context.Preguntas.RemoveRange(preguntas);

                _context.Evaluaciones.Remove(evaluacion);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
