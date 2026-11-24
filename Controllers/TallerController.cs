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
        return View();
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

        return View(BD.ObtenerInscripciones(idUsuario));
    }
}
