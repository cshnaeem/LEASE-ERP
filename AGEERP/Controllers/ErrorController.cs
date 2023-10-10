using System.Web.Mvc;

namespace AGEERP.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult NotFound()
        {
            return View();
        }
        public ActionResult SomethingWentWrong()
        {
            return View();
        }
    }
}