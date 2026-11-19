using Microsoft.AspNetCore.Mvc;

namespace TallerescExtracurriculares.Controllers;

public class CuentaController : Controller
{
    [HttpGet]
    public IActionResult Registro()
    {
        return View();
    }
}
