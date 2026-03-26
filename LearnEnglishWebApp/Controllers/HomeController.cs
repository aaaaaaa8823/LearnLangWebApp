using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LearnEnglishWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.ShowSidebar = false;
            return View();
        }
        
        public IActionResult Me()
        {
            ViewBag.ShowSidebar = true;
            return View();
        }

        public IActionResult Dictionary()
        {
            ViewBag.ShowSidebar = true;
            return View();
        }

        public IActionResult Tests()
        {
            ViewBag.ShowSidebar = true;
            return View();
        }

        public IActionResult Lessons()
        {
            ViewBag.ShowSidebar = true;
            return View();
        }

        public IActionResult Account()
        {
            ViewBag.ShowSidebar = true;
            return View();
        }

        public IActionResult Error()
        {
            ViewBag.ShowSidebar = false;
            return View();
        }
    }
}