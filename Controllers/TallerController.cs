using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TallerescExtracurriculares.Models;

namespace TallerescExtracurriculares.Controllers;

public class TallerController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (HttpContext.Session.GetString("IdUsuario") is null)
        {
            context.Result = RedirectToAction("Login", "Cuenta");
            return;
        }

        base.OnActionExecuting(context);
    }

    public IActionResult Catalogo()
    {
        List<dynamic> talleres = BD.ObtenerTalleres(0);
        ViewBag.HayMas = BD.HayMasTalleres(talleres.Count);
        return View(talleres);
    }

    [HttpGet]
    public IActionResult VerMas(int cantidadMostrada)
    {
        if (cantidadMostrada < 0)
        {
            return BadRequest();
        }

        List<dynamic> talleres = BD.ObtenerTalleres(cantidadMostrada);
        var talleresJson = talleres.Select(taller => new
        {
            id = (int)taller.Id,
            nombre = (string)taller.Nombre,
            imagen = (string)taller.Imagen,
            nombreCategoria = (string)taller.NombreCategoria
        });
        bool hayMas = BD.HayMasTalleres(cantidadMostrada + talleres.Count);

        return Json(new { talleres = talleresJson, hayMas });
    }

    public IActionResult Detalle(int id)
    {
        dynamic? taller = BD.ObtenerTaller(id);
        if (taller is null)
        {
            return RedirectToAction(nameof(Catalogo));
        }

        ViewBag.Taller = taller;
        ViewBag.Encuentros = BD.ObtenerEncuentrosFuturos((int)taller.Id);
        ViewBag.ErrorInscripcion = TempData["ErrorInscripcion"];
        ViewBag.MensajeInscripcion = TempData["MensajeInscripcion"];
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Inscribirse(int idEncuentro)
    {
        if (!int.TryParse(HttpContext.Session.GetString("IdUsuario"), out int idUsuario))
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Cuenta");
        }

        dynamic? encuentro = BD.ObtenerDatosEncuentro(idEncuentro);
        if (encuentro is null)
        {
            return NotFound();
        }

        DateTime fechaHoraEncuentro = (DateTime)encuentro.FechaHora;
        int diasDesdeLunes = ((int)fechaHoraEncuentro.DayOfWeek + 6) % 7;
        DateTime lunes = fechaHoraEncuentro.Date.AddDays(-diasDesdeLunes);
        DateTime domingo = lunes.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);

        int inscripcionesActivas = BD.ContarInscripcionesActivas(idEncuentro);
        if (inscripcionesActivas >= (int)encuentro.Cupo)
        {
            TempData["ErrorInscripcion"] = "El encuentro ya no tiene lugares disponibles.";
        }
        else if (BD.ContarInscripcionesDeLaSemana(idUsuario, lunes, domingo) >= 2)
        {
            TempData["ErrorInscripcion"] =
                "Ya tenés 2 encuentros activos inscriptos durante esa semana. El máximo permitido es 2.";
        }
        else if (!BD.RegistrarInscripcion(idEncuentro, idUsuario))
        {
            TempData["ErrorInscripcion"] = "El encuentro ya no tiene lugares disponibles.";
        }
        else
        {
            TempData["MensajeInscripcion"] = "Te inscribiste correctamente.";
        }

        return RedirectToAction(nameof(Detalle), new { id = (int)encuentro.IdTaller });
    }

    [HttpGet]
    public IActionResult LugaresDisponibles(int idEncuentro)
    {
        int? lugaresDisponibles = BD.ObtenerLugaresDisponibles(idEncuentro);
        if (lugaresDisponibles is null)
        {
            return NotFound();
        }

        return Json(new { lugaresDisponibles });
    }

    public IActionResult MisInscripciones()
    {
        if (!int.TryParse(HttpContext.Session.GetString("IdUsuario"), out int idUsuario))
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Cuenta");
        }

        List<dynamic> inscripciones = BD.ObtenerInscripcionesUsuario(idUsuario);
        DateTime ahora = DateTime.Now;

        ViewBag.Proximas = inscripciones
            .Where(inscripcion =>
                (DateTime)inscripcion.FechaHora > ahora &&
                !(bool)inscripcion.Cancelado)
            .ToList();
        ViewBag.Pasadas = inscripciones
            .Where(inscripcion =>
                (DateTime)inscripcion.FechaHora <= ahora ||
                (bool)inscripcion.Cancelado)
            .ToList();

        return View();
    }
}
