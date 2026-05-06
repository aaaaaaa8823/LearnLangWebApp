using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearnEnglishWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.ShowSidebar = false;
            return View();
        }

        [Authorize]
        public IActionResult Me()
        {
            ViewBag.ShowSidebar = true;
            return View();
        }

        [Authorize]
        public IActionResult Dictionary()
        {
            ViewBag.ShowSidebar = true;
            return View();
        }

        [Authorize]
        public IActionResult Tests()
        {
            ViewBag.ShowSidebar = true;
            return View();
        }

        [Authorize]
        public IActionResult Lessons()
        {

            ViewBag.ShowSidebar = true;
            return View();
        }

        [Authorize]
        public IActionResult Saved()
        {
            ViewBag.ShowSidebar = true;
            return View();
        }

        [Authorize]
        public IActionResult Account()
        {

            ViewBag.ShowSidebar = true;
            return View();
        }

        public IActionResult LessonDetail()
        {
            ViewBag.ShowSidebar = true;
            return View();
        }

        public IActionResult TestDetail()
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