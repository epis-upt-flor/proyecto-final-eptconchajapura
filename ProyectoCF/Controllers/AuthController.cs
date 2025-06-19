using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ProyectoCF.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProyectoCF.Controllers
{
    public class AuthController : Controller
    {
        private readonly Connection _context;

        public AuthController(Connection context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string contrasena)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email && u.Contrasena == contrasena);
            if (usuario != null)
            {
                HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
                HttpContext.Session.SetString("Rol", usuario.Rol);
                HttpContext.Session.SetString("Nombre", usuario.Nombre);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Usuario o contraseña incorrectos.";
            return View();
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Auth", null, Request.Scheme);
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, "Google");
        }


        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("Google");

            if (!authenticateResult.Succeeded)
            {
                return RedirectToAction("Login");
            }

            var emailClaim = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var nombreClaim = authenticateResult.Principal.FindFirst(ClaimTypes.GivenName)?.Value;
            var apellidoClaim = authenticateResult.Principal.FindFirst(ClaimTypes.Surname)?.Value;

            if (emailClaim == null)
            {
                return RedirectToAction("Login");
            }

            // Verificar si el usuario ya existe
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == emailClaim);

            if (usuario == null)
            {
                usuario = new Usuario
                {
                    Nombre = nombreClaim ?? "Sin nombre",
                    Apellido = apellidoClaim ?? "Sin apellido",
                    Email = emailClaim,
                    Contrasena = "GoogleAuth",
                    Rol = "Estudiante"
                };

                _context.Usuarios.Add(usuario);
                _context.SaveChanges();
            }

            HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
            HttpContext.Session.SetString("Rol", usuario.Rol);
            HttpContext.Session.SetString("Nombre", usuario.Nombre);

            return RedirectToAction("Index", "Home");
        }
    }
}
