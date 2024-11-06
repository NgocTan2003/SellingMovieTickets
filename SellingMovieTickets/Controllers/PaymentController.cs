using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.ViewModels.Payments;
using SellingMovieTickets.Repository;

namespace SellingMovieTickets.Controllers
{
    public class PaymentController : Controller
    {
        private readonly DataContext _context;
        public PaymentController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] PaymentInfo paymentInfo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var showTime = await _context.CinemaShowTimes.Where(x => x.Id == paymentInfo.ShowTimeId).Include(x => x.Movie).Include(x => x.Room).FirstOrDefaultAsync();
                    var seatNames = string.Join(", ", paymentInfo.OrderDetails.Select(seatInfo => seatInfo.SeatNumber));

                    var ticket = new TicketModel
                    {
                        NameMovie = showTime.Movie.Name,
                        TicketCode = Guid.NewGuid().ToString(),
                        StartShowTime = showTime.StartShowTime, // Thay bằng thời gian thực tế
                        PaymentTime = DateTime.Now,
                        ConcessionAmount = 0,
                        DiscountAmount = 0,
                        TotalAmount = (decimal)paymentInfo.TotalAmount,
                        PaymentAmount = (decimal)paymentInfo.TotalAmount,
                        SeatNames = seatNames,
                        RoomNumber = showTime.Room.RoomNumber,
                        CreateDate = DateTime.Now,
                    };
                    await _context.Tickets.AddAsync(ticket);
                    await _context.SaveChangesAsync();

                    // Khởi tạo và lưu bản ghi Order
                    var order = new OrderModel
                    {
                        OrderDate = DateTime.Now,
                        PaymentType = paymentInfo.PaymentType,
                        CinemaShowTimeId = paymentInfo.ShowTimeId,
                        TotalAmount = (decimal)paymentInfo.TotalAmount,
                        CreateDate = DateTime.Now,
                        CustomerManagementId = 1 // Thay bằng ID khách hàng hiện tại
                    };
                    await _context.Orders.AddAsync(order);
                    await _context.SaveChangesAsync();

                    // Lấy OrderId vừa tạo để dùng cho OrderDetail
                    int orderId = order.Id;

                    // Tạo và lưu các bản ghi OrderDetail
                    var orderDetails = paymentInfo.OrderDetails.Select(seatInfo => new OrderDetailModel
                    {
                        OrderId = orderId,
                        SeatNumber = seatInfo.SeatNumber,
                        Price = seatInfo.Price,
                        CreateDate = DateTime.Now
                    }).ToList();

                    await _context.OrderDetails.AddRangeAsync(orderDetails);
                    await _context.SaveChangesAsync();



                    return Ok(new { success = true });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Có lỗi xảy ra khi lưu thông tin thanh toán.");
                }
            }

            return BadRequest(ModelState);
        }


    }
}
