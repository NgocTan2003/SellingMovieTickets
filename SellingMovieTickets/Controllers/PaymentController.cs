using Microsoft.AspNetCore.Mvc;

namespace SellingMovieTickets.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
