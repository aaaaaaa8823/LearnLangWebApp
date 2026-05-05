using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnEnglishWebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.ShowSidebar = true;
            return View(); 
        }

        public IActionResult ManageLessons()
        {
            ViewBag.ShowSidebar = true;
            return View();
        }

        public IActionResult ManageTests()
        {
            ViewBag.ShowSidebar = true;
            return View();
        }

        public IActionResult ManageWords()
        {
            ViewBag.ShowSidebar = true;
            return View();
        }
    }
}