using System.Web.Mvc;

namespace Obuchalkin.Controllers
{
    [Authorize]
    public class CoursController : Controller
    {
        // GET: Cours/Index
        public ActionResult Index()
        {
            return View();
        }
    }
}