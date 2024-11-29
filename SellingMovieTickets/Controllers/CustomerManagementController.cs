using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Models.ViewModels.AccountManagement;
using SellingMovieTickets.Repository;
using System.Security.Claims;

namespace SellingMovieTickets.Controllers
{
    public class CustomerManagementController : Controller
    {
        private DataContext _dataContext;

        public CustomerManagementController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IActionResult> Index()
        {
            var idCustomer = User.FindFirstValue(ClaimUserLogin.Id);
            var user = await _dataContext.Users.Where(x => x.Id == idCustomer).FirstOrDefaultAsync();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(AppUserModel model)
        {
            if (model != null)
            {
                var a = await _dataContext.Users.ToListAsync();
                var user = await _dataContext.Users.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                user.LastName = model.LastName;
                user.FirstName = model.FirstName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                _dataContext.Users.Update(user);

                await _dataContext.SaveChangesAsync();
                TempData["Success"] = "Cập nhật thông tin thành công.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Cập nhật thông tin thất bại.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetInfoHistoryTicket()
        {
            var idCustomer = User.FindFirstValue(ClaimUserLogin.Id);

            var tickets = await _dataContext.Orders
                .Where(o => o.CustomerManagement.UserId == idCustomer && o.StatusOrder == "00")
                .Include(o => o.OrderDetails)
                .Include(o => o.CinemaShowTime.Movie)
                .ToListAsync();

            var historyTickets = tickets.Select(order => new HistoryTicket
            {
                TradingDate = order.CreateDate,
                NameMovie = order.CinemaShowTime.Movie.Name,
                NumberOfTickets = order.OrderDetails.Count,
                TotalAmount = order.OrderDetails.Sum(detail => detail.Price)
            }).ToList();

            return Json(historyTickets);
        }

        [HttpGet]
        public async Task<IActionResult> GetInfoHistoryPoint()
        {
            var idCustomer = User.FindFirstValue(ClaimUserLogin.Id);
            //var historyPoints = await _dataContext.CustomerPointsHistories.Include(x=>x.Customer).Where(x => x.Customer.UserId == idCustomer).ToListAsync();
            var historyPoints = await _dataContext.CustomerPointsHistories
                    .Include(x => x.Customer)
                    .Include(x => x.Order)
                    .Where(x => x.Customer.UserId == idCustomer)
                    .Select(x => new
                    {
                        x.Id,
                        x.PointsChanged,
                        x.TransactionDate,
                        x.PointChangeStatus,
                        x.Order.OrderCode,
                    })
                    .ToListAsync();
            return Json(historyPoints);
        }
    }
}
