using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CarAds.Models;

namespace CarAds.Controllers
{
    public class UserController : Controller
    {

        private UserManager<ApplicationUser> _userManager;

        public UserController()
        {
            // Constructor logic can be added here if needed
        }
        public IActionResult Create()
        {
            return View();
        }
    }
}
