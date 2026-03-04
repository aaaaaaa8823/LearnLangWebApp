using Microsoft.AspNetCore.Mvc;

namespace LearnEnglishWebApp.Controllers
{
    public class HomeController: Controller 
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Me()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
