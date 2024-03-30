using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers;

public class UIController : Controller
{
    // [Route("/")]
    public IActionResult Index() {
        return View("/Views/Home/Index.cshtml");
    }
}