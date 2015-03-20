using Microsoft.AspNet.Mvc;

namespace PortfolioOne.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}