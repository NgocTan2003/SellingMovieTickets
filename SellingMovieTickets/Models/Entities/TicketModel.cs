using SellingMovieTickets.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Models.Entities
{
    public class TicketModel : CommonAbstract, CommonPayment
    {
        [Key]
        public int Id { get; set; }
        public string NameMovie { get; set; }
        public string TicketCode { get; set; }
        public DateTime StartShowTime { get; set; }
        public DateTime PaymentTime { get; set; }

        public decimal ConcessionAmount { get; set; } // Số tiền phụ phí bỏng nước
        public decimal TotalAmount { get; set; }      // Tổng tiền
        public decimal DiscountAmount { get; set; }   // Số tiền giảm giá
        public decimal PaymentAmount { get; set; }    // Số tiền phải thanh toán

        public string SeatNames { get; set; }
        public string RoomNumber { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
