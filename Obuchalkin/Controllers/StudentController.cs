using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Obuchalkin.Models;
using Newtonsoft.Json;

namespace Obuchalkin.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private ObuchalkinEntities db = new ObuchalkinEntities();

        private string GetCoursesFilePath()
        {
            return Server.MapPath("~/App_Data/Courses.json");
        }

        private List<Course> LoadCourses()
        {
            string filePath = GetCoursesFilePath();
            if (!System.IO.File.Exists(filePath))
            {
                return new List<Course>();
            }
            string json = System.IO.File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Course>>(json) ?? new List<Course>();
        }

        public ActionResult Index()
        {
            var user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            if (user == null || user.Position != "Обучающийся")
            {
                return RedirectToAction("Index", "Authorization");
            }

            var courses = LoadCourses();
            var progresses = db.Progresses.Where(p => p.Login == user.Login).ToList();
            var results = db.Results.Where(r => r.Login == user.Login).ToList();

            var viewModel = new StudentIndexViewModel
            {
                Courses = courses,
                Progresses = progresses,
                Results = results
            };

            return View(viewModel);
        }

        public ActionResult Course(int? id = null)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            var user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            if (user == null || user.Position != "Обучающийся")
            {
                return RedirectToAction("Index", "Authorization");
            }

            var courses = LoadCourses();
            var course = courses.FirstOrDefault(c => c.ID == id.Value);
            if (course == null)
            {
                return HttpNotFound();
            }

            var progress = db.Progresses.FirstOrDefault(p => p.Login == user.Login && p.Course == id.Value.ToString())
                ?? new Progress { Login = user.Login, Course = id.Value.ToString(), Section = "1", Page = 1, TestCompleted = false };
            var result = db.Results.FirstOrDefault(r => r.Login == user.Login && r.Course == id.Value.ToString());
            int attemptsUsed = result?.AttemptsUsed ?? 0;

            var viewModel = new StudentCourseViewModel
            {
                Course = course,
                CurrentPage = progress.Page.HasValue ? progress.Page.Value : 1,
                Progress = progress,
                AttemptsUsed = attemptsUsed
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SaveProgress(int courseId, int pageNumber, bool testCompleted = false)
        {
            var user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            if (user == null || user.Position != "Обучающийся")
            {
                return Json(new { success = false, message = "Неавторизован" });
            }

            var progress = db.Progresses.FirstOrDefault(p => p.Login == user.Login && p.Course == courseId.ToString());
            if (progress == null)
            {
                progress = new Progress
                {
                    Login = user.Login,
                    Course = courseId.ToString(),
                    Section = "1",
                    Page = pageNumber,
                    TestCompleted = testCompleted
                };
                db.Progresses.Add(progress);
            }
            else
            {
                progress.Page = pageNumber;
                progress.TestCompleted = testCompleted;
            }

            db.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult SubmitTest(int courseId, string[] answers)
        {
            try
            {
                var user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
                if (user == null || user.Position != "Обучающийся")
                {
                    return Json(new { success = false, message = "Неавторизован" });
                }

                var courses = LoadCourses();
                var course = courses.FirstOrDefault(c => c.ID == courseId);
                if (course == null || course.Tests.Count == 0)
                {
                    return Json(new { success = false, message = "Курс или тесты не найдены" });
                }

                var result = db.Results.FirstOrDefault(r => r.Login == user.Login && r.Course == courseId.ToString());
                int attemptsUsed = result?.AttemptsUsed ?? 0;
                if (attemptsUsed >= 2)
                {
                    return Json(new { success = false, message = "Вы исчерпали 2 попытки. Обратитесь к тренеру для сброса." });
                }

                int correctAnswers = 0;
                for (int i = 0; i < course.Tests.Count && i < answers.Length; i++)
                {
                    if (course.Tests[i].CorrectAnswer == answers[i])
                    {
                        correctAnswers++;
                    }
                }
                int score = (int)((correctAnswers / (float)course.Tests.Count) * 100);

                if (result == null)
                {
                    result = new Result
                    {
                        Login = user.Login,
                        Course = courseId.ToString(),
                        AttemptsUsed = 1,
                        BestResult = score
                    };
                    db.Results.Add(result);
                }
                else
                {
                    result.AttemptsUsed = attemptsUsed + 1;
                    result.BestResult = Math.Max(result.BestResult ?? 0, score);
                }

                var progress = db.Progresses.FirstOrDefault(p => p.Login == user.Login && p.Course == courseId.ToString());
                if (progress == null)
                {
                    progress = new Progress
                    {
                        Login = user.Login,
                        Course = courseId.ToString(),
                        Section = "1",
                        Page = course.Pages.Count + 1,
                        TestCompleted = true
                    };
                    db.Progresses.Add(progress);
                }
                else
                {
                    progress.TestCompleted = true;
                }

                db.SaveChanges();
                return Json(new { success = true, score = score, message = "Тест успешно сохранён" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ошибка сервера: " + ex.Message });
            }
        }

        [Authorize(Roles = "Тренер")]
        [HttpPost]
        public ActionResult ResetTestAttempts(string login, int courseId)
        {
            var result = db.Results.FirstOrDefault(r => r.Login == login && r.Course == courseId.ToString());
            if (result != null)
            {
                result.AttemptsUsed = 0;
                var progress = db.Progresses.FirstOrDefault(p => p.Login == login && p.Course == courseId.ToString());
                if (progress != null)
                {
                    progress.TestCompleted = false;
                }
                db.SaveChanges();
                return Json(new { success = true, message = "Попытки сброшены" });
            }
            return Json(new { success = false, message = "Результат не найден" });
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