using Microsoft.AspNetCore.Mvc;
using TallerescExtracurriculares.Models;

namespace TallerescExtracurriculares.Controllers;

public class CuentaController : Controller
{
    [HttpGet]
    public IActionResult Registro()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Registro(Usuario usuario)
    {
        if (string.IsNullOrWhiteSpace(usuario.NombreUsuario))
        {
            return MostrarErrorRegistro(usuario, "El nombre de usuario es obligatorio.");
        }
        if (usuario.NombreUsuario.Length > 50)
        {
            return MostrarErrorRegistro(usuario, "El nombre de usuario no puede superar los 50 caracteres.");
        }
        if (string.IsNullOrWhiteSpace(usuario.Contrasena))
        {
            return MostrarErrorRegistro(usuario, "La contraseña es obligatoria.");
        }
        if (usuario.Contrasena.Length > 50)
        {
            return MostrarErrorRegistro(usuario, "La contraseña no puede superar los 50 caracteres.");
        }
        if (string.IsNullOrWhiteSpace(usuario.Nombre))
        {
            return MostrarErrorRegistro(usuario, "El nombre es obligatorio.");
        }
        if (usuario.Nombre.Length > 50)
        {
            return MostrarErrorRegistro(usuario, "El nombre no puede superar los 50 caracteres.");
        }
        if (string.IsNullOrWhiteSpace(usuario.Apellido))
        {
            return MostrarErrorRegistro(usuario, "El apellido es obligatorio.");
        }
        if (usuario.Apellido.Length > 50)
        {
            return MostrarErrorRegistro(usuario, "El apellido no puede superar los 50 caracteres.");
        }
        if (BD.ExisteNombreUsuario(usuario.NombreUsuario))
        {
            return MostrarErrorRegistro(usuario, "Ese nombre de usuario ya está registrado.");
        }

        BD.RegistrarUsuario(usuario);
        return RedirectToAction("Login");
    }

    private IActionResult MostrarErrorRegistro(Usuario usuario, string mensaje)
    {
        ViewBag.Error = mensaje;
        return View("Registro", usuario);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(string NombreUsuario, string Contrasena)
    {
        if (string.IsNullOrWhiteSpace(NombreUsuario) ||
            string.IsNullOrWhiteSpace(Contrasena))
        {
            ViewBag.Error = "Usuario o contraseña incorrectos";
            return View();
        }

        Usuario? usuario = BD.Login(NombreUsuario, Contrasena);
        if (usuario is null)
        {
            ViewBag.Error = "Usuario o contraseña incorrectos";
            return View();
        }

        HttpContext.Session.SetString("IdUsuario", usuario.Id.ToString());
        return RedirectToAction("Catalogo", "Taller");
    }

    public IActionResult CerrarSesion()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
