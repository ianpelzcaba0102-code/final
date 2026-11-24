using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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
    public IActionResult Registro(Usuario usuario, string repetirContrasena)
    {
        usuario.Email = usuario.Email?.Trim() ?? string.Empty;
        usuario.NombreUsuario = usuario.NombreUsuario?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(usuario.Email))
        {
            return MostrarErrorRegistro(usuario, "El correo electrónico es obligatorio.");
        }
        if (usuario.Email.Length > 254)
        {
            return MostrarErrorRegistro(usuario, "El correo electrónico no puede superar los 254 caracteres.");
        }
        if (!new EmailAddressAttribute().IsValid(usuario.Email))
        {
            return MostrarErrorRegistro(usuario, "Ingresá un correo electrónico válido.");
        }
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
        if (string.IsNullOrWhiteSpace(repetirContrasena) || usuario.Contrasena != repetirContrasena)
        {
            return MostrarErrorRegistro(usuario, "Las contraseñas no coinciden.");
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
        if (BD.ExisteNombreUsuarioOEmail(usuario.NombreUsuario, usuario.Email))
        {
            return MostrarErrorRegistro(usuario, "El nombre de usuario o el mail ya está registrado.");
        }

        BD.RegistrarUsuario(usuario);
        TempData["MensajeLogin"] = "La cuenta se creó correctamente. Ya podés iniciar sesión.";
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
        ViewBag.Error = TempData["ErrorSesion"];
        ViewBag.Mensaje = TempData["MensajeLogin"];
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
