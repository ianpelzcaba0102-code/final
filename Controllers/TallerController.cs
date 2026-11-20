using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
        return View();
    }

    public IActionResult Detalle()
    {
        return View();
    }

    public IActionResult MisInscripciones()
    {
        return View();
    }
}
