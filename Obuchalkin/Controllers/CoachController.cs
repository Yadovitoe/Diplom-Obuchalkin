using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Obuchalkin.Models; // Подключаем пространство имён для модели
using Newtonsoft.Json;

namespace Obuchalkin.Controllers
{
    [Authorize]
    public class CoachController : Controller
    {
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

        private void SaveCourses(List<Course> courses)
        {
            string filePath = GetCoursesFilePath();
            string json = JsonConvert.SerializeObject(courses, Formatting.Indented);
            System.IO.File.WriteAllText(filePath, json);
        }

        private ObuchalkinEntities db = new ObuchalkinEntities();

        public ActionResult Index()
        {
            var user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            if (user == null || user.Position != "Тренер")
            {
                return RedirectToAction("Index", "Authorization");
            }

            var courses = LoadCourses();
            var progresses = db.Progresses.ToList();
            var results = db.Results.ToList();

            var viewModel = new CoachIndexViewModel
            {
                Courses = courses,
                Progresses = progresses,
                Results = results
            };

            return View(viewModel);
        }

        public ActionResult CreateCourse()
        {
            var user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            if (user == null || user.Position != "Тренер")
            {
                return RedirectToAction("Index", "Authorization");
            }
            return View(new Course());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreateCourse(Course course, string[] pageContents, HttpPostedFileBase[] pageImages, string[] testQuestions, string[] testTypes, string[] correctAnswers, string[][] testOptions = null, int? testTimeLimit = null)
        {
            var user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            if (user == null || user.Position != "Тренер")
            {
                return RedirectToAction("Index", "Authorization");
            }

            if (string.IsNullOrEmpty(course.Title) || string.IsNullOrEmpty(course.Description))
            {
                TempData["Error"] = "Название и описание курса обязательны";
                return View(course);
            }

            var courses = LoadCourses();
            course.ID = courses.Any() ? courses.Max(c => c.ID) + 1 : 1;
            course.TestTimeLimit = testTimeLimit;

            if (pageContents != null && pageContents.Length > 0)
            {
                for (int i = 0; i < pageContents.Length; i++)
                {
                    if (string.IsNullOrEmpty(pageContents[i]))
                        continue;

                    var page = new Page
                    {
                        ID = course.Pages.Any() ? course.Pages.Max(p => p.ID) + 1 : 1,
                        Content = pageContents[i]
                    };

                    if (pageImages != null && i < pageImages.Length && pageImages[i] != null && pageImages[i].ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(pageImages[i].FileName);
                        string imagePath = "~/Content/Images/" + course.ID + "_" + page.ID + "_" + fileName;
                        string physicalPath = Server.MapPath(imagePath);
                        pageImages[i].SaveAs(physicalPath);
                        page.ImagePath = imagePath;
                    }

                    course.Pages.Add(page);
                }
            }

            if (testQuestions != null && testQuestions.Length > 0)
            {
                for (int i = 0; i < testQuestions.Length; i++)
                {
                    if (string.IsNullOrEmpty(testQuestions[i]) || string.IsNullOrEmpty(testTypes[i]) || string.IsNullOrEmpty(correctAnswers[i]))
                        continue;

                    var test = new Test
                    {
                        ID = course.Tests.Any() ? course.Tests.Max(t => t.ID) + 1 : 1,
                        Question = testQuestions[i],
                        Type = testTypes[i],
                        CorrectAnswer = correctAnswers[i]
                    };

                    if (test.Type == "MultipleChoice" && testOptions != null && testOptions.Length > i && testOptions[i] != null)
                    {
                        test.Options = testOptions[i].ToList();
                    }

                    course.Tests.Add(test);
                }
            }

            courses.Add(course);
            SaveCourses(courses);
            return RedirectToAction("Index");
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