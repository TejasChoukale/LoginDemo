using Microsoft.AspNetCore.Mvc;
using LoginDemoApp.Data;
using LoginDemoApp.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace LoginDemoApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        private readonly PasswordHasher<User> _passwordHasher;

        public AccountController(AppDbContext db)
        {
            _db = db;
            _passwordHasher = new PasswordHasher<User>();
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(string username, string password)
        {
            // Check duplicate user
            if (_db.Users.Any(u => u.Username == username))
            {
                ViewBag.Error = "Username already exists!";
                return View();
            }

            var user = new User { Username = username };

            // Hash password
            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            _db.Users.Add(user);
            _db.SaveChanges();

            return RedirectToAction("Login");
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                ViewBag.Error = "Invalid username or password!";
                return View();
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Success)
            {
                return Content("Login Successful!");
            }

            ViewBag.Error = "Invalid username or password!";
            return View();
        }
    }
}
