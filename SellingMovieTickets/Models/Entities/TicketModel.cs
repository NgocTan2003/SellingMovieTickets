using System.ComponentModel.DataAnnotations;

namespace SellingMovieTickets.Models.Entities
{
    public class TicketModel : CommonAbstract
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Giá vé không được bỏ trống")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Ngày mua vé không được bỏ trống")]
        public DateTime PurchaseDate { get; set; }
        [Required(ErrorMessage = "Trạng thái thanh toán không được bỏ trống")]
        public string PaymentStatus { get; set; }
        [Required(ErrorMessage = "Hình thức thanh toán không được bỏ trống")]
        public string TicketType { get; set; }

        public int CinemaShowTimeId { get; set; }
        public CinemaShowTimeModel CinemaShowTime { get; set; }

        public string SeatNumber { get; set; }
        public string RoomNumber { get; set; }

        public int UserId { get; set; }
        public AppUserModel AppUser { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        
    }
}
