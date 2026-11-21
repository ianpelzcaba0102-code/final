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
        return View(talleres);
    }

    public IActionResult Detalle()
    {
        return View();
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
