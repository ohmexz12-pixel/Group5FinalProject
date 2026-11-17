using Microsoft.AspNetCore.Mvc;

namespace firstASP.Controllers;

public class GuidelinesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}