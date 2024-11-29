using Microsoft.AspNetCore.Mvc;

namespace SellingMovieTickets.Areas.Admin.Controllers
{
    public class OtherServicesOrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
