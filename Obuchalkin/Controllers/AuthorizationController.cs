using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Obuchalkin.Models;
using System.Security.Cryptography;
using System.Text;

namespace Obuchalkin.Controllers
{
    public class AuthorizationController : Controller
    {
        private ObuchalkinEntities db = new ObuchalkinEntities();

        [AllowAnonymous]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
                if (user != null)
                {
                    switch (user.Position)
                    {
                        case "Обучающийся":
                            return RedirectToAction("Index", "Student");
                        case "Специалист по обучению":
                            return RedirectToAction("Index", "Coach");
                        case "Менеджер":
                            return RedirectToAction("Index", "Manager");
                        default:
                            return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(string username, string password, bool rememberMe = false)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Пожалуйста, заполните все поля";
                return View();
            }

            string hashedPassword = HashPassword(password);
            var user = db.Users.FirstOrDefault(u => u.Login == username);

            if (user != null)
            {
                if (IsPasswordHashed(user.Password))
                {
                    if (user.Password == hashedPassword)
                    {
                        FormsAuthentication.SetAuthCookie(username, rememberMe);
                        return RedirectToRoleDashboard(user.Position);
                    }
                }
                else
                {
                    if (user.Password == password)
                    {
                        user.Password = hashedPassword;
                        db.SaveChanges();
                        FormsAuthentication.SetAuthCookie(username, rememberMe);
                        return RedirectToRoleDashboard(user.Position);
                    }
                }
            }

            ViewBag.Error = "Неверный логин или пароль";
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

        private ActionResult RedirectToRoleDashboard(string role)
        {
            switch (role)
            {
                case "Обучающийся":
                    return RedirectToAction("Index", "Student");
                case "Тренер":
                    return RedirectToAction("Index", "Coach");
                case "Менеджер":
                    return RedirectToAction("Index", "Manager");
                default:
                    return RedirectToAction("Index", "Home");
            }
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

        private bool IsPasswordHashed(string password)
        {
            return password.Length == 64 && password.All(c => "0123456789abcdef".Contains(c));
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