using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LearnEnglishWebApp.Controllers
{
    public class HomeController : Controller
    {
        // Главная страница (авторизация) - без сайдбара
        public IActionResult Index()
        {
            ViewBag.ShowSidebar = false;
            return View();
        }

        //Todo Исправить ошибку с авторизацией но пока отставлю так
        //[Authorize]
        public IActionResult Me()
        {
            ViewBag.ShowSidebar = true;
            return View();
        }

        //[Authorize]
        public IActionResult Dictionary()
        {
            ViewBag.ShowSidebar = true;
            return View();
        }

        //[Authorize]
        public IActionResult Tests()
        {
            ViewBag.ShowSidebar = true;
            return View();
        }

        //[Authorize]
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