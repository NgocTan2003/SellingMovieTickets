using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SellingMovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CustomerManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
