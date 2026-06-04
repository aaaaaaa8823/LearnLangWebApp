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

        public IActionResult ManageContent()
        {
            ViewBag.ShowSidebar = true;
            return View();
        }

        public IActionResult ManageUsers()
        {
            ViewBag.ShowSidebar = true;
            return View();
        }
    }
}