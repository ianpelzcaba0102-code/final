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
            context.ActionDescriptor.RouteValues.TryGetValue(
                "action",
                out string? nombreAccion
            );

            bool esCancelacion = string.Equals(
                nombreAccion,
                nameof(CancelarInscripcion),
                StringComparison.OrdinalIgnoreCase
            );
            bool esFetch = string.Equals(nombreAccion, nameof(VerMas), StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(nombreAccion, nameof(LugaresDisponibles), StringComparison.OrdinalIgnoreCase);

            if (esFetch)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = Json(new
                {
                    ok = false,
                    mensaje = "Tu sesión expiró. Iniciá sesión nuevamente."
                });
                return;
            }

            if (!esCancelacion)
            {
                TempData["ErrorSesion"] = "Tu sesión expiró. Iniciá sesión nuevamente.";
                context.Result = RedirectToAction("Login", "Cuenta");
                return;
            }
        }

        base.OnActionExecuting(context);
    }

    public IActionResult Catalogo()
    {
        List<dynamic> talleres = BD.ObtenerTalleres(0);
        ViewBag.HayMas = BD.HayMasTalleres(talleres.Count);
        ViewBag.ErrorCatalogo = TempData["ErrorCatalogo"];
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
            TempData["ErrorCatalogo"] = "El taller solicitado no existe.";
            return RedirectToAction(nameof(Catalogo));
        }
        if (!(bool)taller.Activo)
        {
            TempData["ErrorCatalogo"] = "Este taller está inactivo y no se puede ver ni reservar.";
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
            TempData["ErrorSesion"] = "Tu sesión expiró. Iniciá sesión nuevamente.";
            return RedirectToAction("Login", "Cuenta");
        }

        dynamic? encuentro = BD.ObtenerDatosEncuentro(idEncuentro);
        if (encuentro is null)
        {
            TempData["ErrorCatalogo"] = "El encuentro seleccionado no existe.";
            return RedirectToAction(nameof(Catalogo));
        }
        if (!(bool)encuentro.Activo)
        {
            TempData["ErrorCatalogo"] = "El taller está inactivo y no se puede reservar.";
            return RedirectToAction(nameof(Catalogo));
        }

        DateTime fechaHoraEncuentro = (DateTime)encuentro.FechaHora;
        if (fechaHoraEncuentro <= DateTime.Now)
        {
            TempData["ErrorInscripcion"] = "No podés inscribirte: el encuentro ya pasó.";
            return RedirectToAction(nameof(Detalle), new { id = (int)encuentro.IdTaller });
        }

        int diasDesdeLunes = ((int)fechaHoraEncuentro.DayOfWeek + 6) % 7;
        DateTime lunes = fechaHoraEncuentro.Date.AddDays(-diasDesdeLunes);
        DateTime domingo = lunes.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);

        if (BD.ExisteInscripcionActiva(idUsuario, idEncuentro))
        {
            TempData["ErrorInscripcion"] = "Ya estás inscripto en este encuentro.";
        }
        else if (BD.ContarInscripcionesActivas(idEncuentro) >= (int)encuentro.Cupo)
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
            Response.StatusCode = StatusCodes.Status404NotFound;
            return Json(new { ok = false, mensaje = "El encuentro seleccionado no existe." });
        }

        return Json(new { lugaresDisponibles });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CancelarInscripcion(int idInscripcion)
    {
        if (!int.TryParse(HttpContext.Session.GetString("IdUsuario"), out int idUsuario))
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Json(new { ok = false, mensaje = "Tu sesión expiró. Iniciá sesión nuevamente." });
        }

        dynamic? inscripcion = BD.ObtenerInscripcionParaCancelar(idInscripcion, idUsuario);
        if (inscripcion is null)
        {
            return Json(new { ok = false, mensaje = "La inscripción no existe o no te pertenece." });
        }
        if ((bool)inscripcion.Cancelado)
        {
            return Json(new { ok = false, mensaje = "La inscripción ya fue cancelada." });
        }
        if ((DateTime)inscripcion.FechaHora <= DateTime.Now)
        {
            return Json(new { ok = false, mensaje = "No se puede cancelar: el encuentro ya comenzó o pasó." });
        }
        if (!BD.CancelarInscripcion(idInscripcion))
        {
            return Json(new { ok = false, mensaje = "La inscripción ya fue cancelada." });
        }

        return Json(new { ok = true, mensaje = "Inscripción cancelada." });
    }

    public IActionResult MisInscripciones()
    {
        if (!int.TryParse(HttpContext.Session.GetString("IdUsuario"), out int idUsuario))
        {
            HttpContext.Session.Clear();
            TempData["ErrorSesion"] = "Tu sesión expiró. Iniciá sesión nuevamente.";
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
