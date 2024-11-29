using Microsoft.AspNetCore.Mvc;

namespace SellingMovieTickets.Areas.Admin.Controllers
{
    public class CustomerPointsHistoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
