using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Obuchalkin.Models;
using System.Security.Cryptography;
using System.Text;

namespace Obuchalkin.Controllers
{
    [Authorize]
    public class ManagerController : Controller
    {
        private ObuchalkinEntities db = new ObuchalkinEntities();

        public ActionResult Index()
        {
            var user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            if (user == null || user.Position != "Менеджер")
            {
                return RedirectToAction("Index", "Authorization");
            }
            var users = db.Users.ToList();
            return View(users);
        }

        [HttpPost]
        public ActionResult Register(string fullName, string login, string password, string role)
        {
            var currentUser = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            if (currentUser == null || currentUser.Position != "Менеджер")
            {
                return RedirectToAction("Index", "Authorization");
            }

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                TempData["Error"] = "Все поля должны быть заполнены";
                return RedirectToAction("Index");
            }

            if (db.Users.Any(u => u.Login == login))
            {
                TempData["Error"] = "Пользователь с таким логином уже существует";
                return RedirectToAction("Index");
            }

            var newUser = new User
            {
                FullName = fullName,
                Login = login,
                Password = HashPassword(password), // Сразу хешируем пароль
                Position = role
            };

            db.Users.Add(newUser);
            db.SaveChanges();

            TempData["Success"] = "Пользователь успешно зарегистрирован";
            return RedirectToAction("Index");
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}