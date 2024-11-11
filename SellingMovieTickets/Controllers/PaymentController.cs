using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Models.ViewModels.CinemaShowTimes;
using SellingMovieTickets.Models.ViewModels.Payments;
using SellingMovieTickets.Repository;
using System.Security.Claims;

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

        public IActionResult Success()
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
                    var userId = User.FindFirstValue(ClaimUserLogin.Id);
                    var customerManagement = await _context.CustomerManagements.Where(x => x.UserId == userId).FirstOrDefaultAsync();
                    var showTime = await _context.CinemaShowTimes.Where(x => x.Id == paymentInfo.ShowTimeId).Include(x => x.Movie).Include(x => x.Room).FirstOrDefaultAsync();
                    var seatNames = string.Join(", ", paymentInfo.OrderSeats.Select(seatInfo => seatInfo.SeatName));

                    // tạo ticket
                    var ticket = new TicketModel
                    {
                        NameMovie = showTime.Movie.Name,
                        TicketCode = Guid.NewGuid().ToString(),
                        StartShowTime = showTime.StartShowTime,
                        PaymentTime = DateTime.Now,
                        ConcessionAmount = paymentInfo.ConcessionAmount,
                        DiscountAmount = paymentInfo.DiscountAmount,
                        TotalAmount = paymentInfo.PaymentAmount,
                        PaymentAmount = paymentInfo.PaymentAmount,
                        SeatNames = seatNames,
                        RoomNumber = showTime.Room.RoomNumber,
                        CreateDate = DateTime.Now,
                    };
                    await _context.Tickets.AddAsync(ticket);
                    await _context.SaveChangesAsync();

                    // tạo order
                    var order = new OrderModel();
                    order.OrderDate = DateTime.Now;
                    order.PaymentType = paymentInfo.PaymentType;
                    order.NumberOfTickets = paymentInfo.OrderSeats.Count();
                    order.TotalAmount = paymentInfo.PaymentAmount;
                    order.TicketId = ticket.Id;
                    order.CinemaShowTimeId = paymentInfo.ShowTimeId;
                    order.CustomerManagementId = customerManagement.Id;
                    if (paymentInfo.PromotionId != null)
                    {
                        order.PromotionId = paymentInfo.PromotionId;
                    }
                    order.CreateDate = DateTime.Now;
                    await _context.Orders.AddAsync(order);
                    await _context.SaveChangesAsync();

                    // tạo orderDetail
                    var orderDetails = paymentInfo.OrderSeats.Select(seatInfo => new OrderDetailModel
                    {
                        OrderId = order.Id,
                        SeatNumber = seatInfo.SeatName,
                        Price = seatInfo.Price,
                        CreateDate = DateTime.Now
                    }).ToList();
                    await _context.OrderDetails.AddRangeAsync(orderDetails);
                    await _context.SaveChangesAsync();

                    if (paymentInfo.OtherServices != null)
                    {
                        var orderServiceOrders = new List<OtherServicesOrderModel>();
                        foreach (var service in paymentInfo.OtherServices)
                        {
                            var otherService = await _context.OtherServices.FindAsync(service.Id);
                            if (otherService != null)
                            {
                                var totalAmount = otherService.Price * service.Quantity;
                                orderServiceOrders.Add(new OtherServicesOrderModel
                                {
                                    OrderId = order.Id,
                                    OtherServicesId = service.Id,
                                    Quantity = service.Quantity,
                                    TotalAmount = totalAmount,
                                    CreateDate = DateTime.Now
                                });
                            }
                        }
                        await _context.OtherServicesOrders.AddRangeAsync(orderServiceOrders);
                        await _context.SaveChangesAsync();
                    }

                    // cập nhật trạng thái của ghế được chọn
                    var seatsToUpdate = _context.Seats
                        .Where(seat => seat.CinemaShowTimeId == paymentInfo.ShowTimeId)
                        .AsEnumerable()  
                        .Where(seat => paymentInfo.OrderSeats.Any(s => s.SeatName == seat.SeatNumber)) 
                        .ToList();  


                    foreach (var seat in seatsToUpdate)
                    {
                        seat.IsAvailable = false;
                    }

                    _context.Seats.UpdateRange(seatsToUpdate);
                    await _context.SaveChangesAsync();

                    // cập nhật điểm thưởng
                    var updateCustomer = await _context.CustomerManagements.Where(x => x.UserId == userId).FirstOrDefaultAsync();
                    updateCustomer.TotalTicketsPurchased += paymentInfo.OrderSeats.Count();
                    updateCustomer.TotalSpent += paymentInfo.PaymentAmount;
                    updateCustomer.CurrentPointsBalance += 10;
                    _context.CustomerManagements.Update(updateCustomer);
                    await _context.SaveChangesAsync();

                    // cập nhật lịch sử điểm thưởng
                    var customerPointsHistory = new CustomerPointsHistoryModel
                    {
                        CustomerId = customerManagement.Id,
                        PointsChanged = 10,
                        TransactionDate = DateTime.Now,
                        PointChangeStatus = PointChangeStatus.BuyTicket,
                        CreateDate = DateTime.Now,
                    };
                    await _context.CustomerPointsHistories.AddAsync(customerPointsHistory);
                    await _context.SaveChangesAsync();

                    return Ok(new { success = true });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi lưu thông tin thanh toán.", error = ex.Message, stackTrace = ex.StackTrace });
                }
            }
            return BadRequest(ModelState);
        }



    }
}
