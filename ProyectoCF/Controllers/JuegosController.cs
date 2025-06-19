using Microsoft.AspNetCore.Mvc;

namespace ProyectoCF.Controllers
{
    public class JuegosController : Controller
    {
        private readonly List<string> _juegos = new()
        {
            @"<iframe style='max-width:100%' src='https://wordwall.net/es/embed/24b94cd4957e4dac955cfa4729d527a7?themeId=1&templateId=46&fontStackId=0' width='500' height='380' frameborder='0' allowfullscreen></iframe>",
            
            @"<iframe style='max-width:100%' src='https://wordwall.net/es/embed/24b94cd4957e4dac955cfa4729d527a7?themeId=46&templateId=46&fontStackId=0' width='500' height='380' frameborder='0' allowfullscreen></iframe>",
            
            @"<iframe style='max-width:100%' src='https://wordwall.net/es/embed/24b94cd4957e4dac955cfa4729d527a7?themeId=52&templateId=46&fontStackId=0' width='500' height='380' frameborder='0' allowfullscreen></iframe>",
            
            @"<iframe style='max-width:100%' src='https://wordwall.net/es/embed/1b87c3fb4f7f4588a380d5e105e563c3?themeId=65&templateId=30&fontStackId=0' width='500' height='380' frameborder='0' allowfullscreen></iframe>"
        };

        public IActionResult Index(int? juegoId = 0)
        {
            int id = juegoId ?? 0;
            if (id < 0) id = 0;
            if (id >= _juegos.Count) id = _juegos.Count - 1;

            ViewBag.Juegos = _juegos;
            ViewBag.JuegoActual = id;
            ViewBag.TotalJuegos = _juegos.Count;

            return View();
        }
    }
}